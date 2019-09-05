#   file:   pokeguesser.py
#   desc:   Discord bot that guesses pokemon by image

import argparse
import asyncio

import aiohttp              # pip install discord
import discord

import numpy as np          # pip install numpy
from cv2 import cv2         # pip install opencv-python
from joblib import load     # pip install joblib
from sklearn import svm     # pip install scikit-learn

#   Parse cmd line arguments
parser = argparse.ArgumentParser(description='Discord bot that guesses pokemon by image')

parser.add_argument('--auth', '-a', action='store', type=str, required=True, 
    help='Discord auth token. Google or ask me how to obtain your own. Specify a bot token with -b')

parser.add_argument('--isBot', '-b', action='store_true', 
    help='Flag to indicate token argument is a bot token')

parser.add_argument('--target', '-t', action='store', type=str, default='Pokécord#4503', 
    help='Target Pokecord bot\'s username and tag. default: "Pokécord#4503"')

parser.add_argument('--delay', '-d', action='store', type=int, default=10, 
    help='The delay in seconds to wait before guessing. default: 10')

parser.add_argument('--hints', '-H', action='store_true', 
    help='Flag to have the bot print the guess as a hint before guessing')

args = parser.parse_args()

AUTH_TOKEN = args.auth
IS_BOT = args.isBot
POKECORD_ID = args.target
DELAY = args.delay if args.delay >= 0 else 0
HINTS_ENABLED = args.hints

#   Load svm model (0.20.2 works on 0.20.3 as far as I can tell)
clf = load('100x100model.joblib')

#   Client for GET requests (ignore the depreciated warning)
http_client = aiohttp.ClientSession()

#   Derived from discord.Client. Listens for Pokecord bot to post a new image
#   before downloading and classifying the image as a pokemon.
class PokeGuesser(discord.Client):

    async def on_ready(self):
        print('Connected to Discord as: ' + str(self.user))
    
    #   Listens to all message from all servers user is a member in.
    #   Once Pokecord bot posts a pokemon image:
    #       - download image into np array
    #       - resize image to 100x100
    #       - classify image using svm model
    #       - send the hint
    #       - wait for the delay
    #       - send the catch command
    async def on_message(self, message):
        msg_author = str(message.author)
        print(msg_author + ': ' + message.content) # might as well do some creepy logging
        if msg_author == POKECORD_ID and message.embeds:
            try: 
                url = message.embeds[0].image.url
                split_url = url.replace('/','.').split('.')
                if 'PokecordSpawn' in split_url:
                    response = await http_client.get(url)
                    byte_stream = await response.content.read()
                    img_array = np.asarray(bytearray(byte_stream), dtype='uint8')
                    img = cv2.imdecode(img_array, cv2.IMREAD_ANYCOLOR)
                    img = cv2.resize(img, dsize=(100, 100), interpolation=cv2.INTER_CUBIC)
                    img = np.array([img])
                    img = img.reshape(img.shape[0], -1)
                    prediction = clf.predict(img)[0]
                    if HINTS_ENABLED and DELAY > 0:
                        hidden_chars = '* ' * (len(prediction)-2)
                        hint_name = str(prediction[0] + ' ' + hidden_chars + prediction[-1])
                        await message.channel.send('I think this might be ' + 
                            hint_name + '. ' + str(DELAY) + ' seconds until I try to catch it.')
                    await asyncio.sleep(DELAY)
                    await message.channel.send('p!catch ' + prediction)
            except Exception:
                return        
        
client = PokeGuesser(cache_auth=False)
client.run(AUTH_TOKEN,bot=IS_BOT)
