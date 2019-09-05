import boto3
import json

from joblib import load
from os import environ, remove
from sklearn import svm

MODEL_BUCKET_NAME       = environ['MODEL_BUCKETNAME']
MODEL_OBJECT_NAME       = environ['MODEL_OBJECTNAME']
MODEL_FILE_PATH         = '/tmp/' + MODEL_BUCKET_NAME

COMPOSITION_MAP_STRING  = environ['COMPOSITION_MAP_STRING']
PACKAGING_MAP_STRING    = environ['PACKAGING_MAP_STRING']

s3 = boto3.client('s3')

# Download model into Lambda /tmp/ directory (max storage 512 mb)
# Load into memory and delete from directory
s3.download_file(MODEL_BUCKET_NAME, MODEL_OBJECT_NAME, MODEL_FILE_PATH)
model = load(MODEL_FILE_PATH)
remove(MODEL_FILE_PATH)

CompositionToIntMap = json.loads(COMPOSITION_MAP_STRING)
PackagingToIntMap   = json.loads(PACKAGING_MAP_STRING)

# Converts passed data into integers and returns model prediction 
# and probability (confidence in prediction)
#
# params:
#   data: JSON object in the format:
#      {  
#       'Composition' : string,    
#       'Packaging': string, 
#       'WattHours': number 
#      }
#   context: Lambda passed context
#
# returns: JSON object in the format:
#      {
#       'Status': number, 
#       'Message': string, 
#       'Prediction': string, 
#       'Probability': number
#      }
def handler(data, context):
    try:
        composition = CompositionToIntMap[data['Composition']]
        packaging   = PackagingToIntMap[data['Packaging']]
        wattHours   = data['WattHours']
    except Exception as exc:
        return {
                'Status': 400, 
                'Message': 'InvalidParameterException: ' + str(exc), 
                'Prediction': '', 
                'Probability': ''
               }
    try:
        prediction  = model.predict([[composition, packaging, wattHours]])[0]
        probability = model.predict_proba([[composition, packaging, wattHours]])
    except Exception as exc:
        return {
                'Status': 400,
                'Message': 'ModelException: ' + str(exc), 
                'Prediction': '', 
                'Probability': ''
               }
    return {
            'Status': 200, 
            'Message': 'Success', 
            'Prediction': prediction, 
            'Probability': max(probability[0])
           }