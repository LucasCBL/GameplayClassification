import tensorflow as tf

from tensorflow import feature_column
from tensorflow.keras import layers
from sklearn.model_selection import train_test_split

import os
import numpy as np
import matplotlib.pyplot as plt
import json
import pandas as pd
import pathlib

import numpy as np
import pandas as pd


from tensorflow import keras
from tensorflow.keras import layers
from sklearn.metrics import confusion_matrix
from tensorflow.keras.layers.experimental import preprocessing

def load_data():
    dataset = {}
    os.chdir("PLAYER_DATA")
    # cambiamos la carpeta de trabajo

    # lista de carpetas con datos, estas se llaman con la id del bot
    folders = os.listdir()
    for bot_data in folders:

        bot_type = bot_data
        #id del bot, innecesaria ahora mismo
        #bot_id = split_name[bot + 1]
        if bot_type not in dataset.keys():
            dataset[bot_type] = []
        # nos metemos en la carpeta base de datos del bot
        os.chdir(bot_data)
        levels = os.listdir()
        data = []
        for level in levels:
            level_data = []
            os.chdir(level)
            logs = os.listdir() # todos los ficheros de datos de ese nivel
            for log in logs:
                data_log = ''
                with open(log) as infile:
                    data_log = json.loads(infile.read())
                level_data.append(data_log) # log en formato json se añade a los logs del nivel
            data.append(level_data) #añadimos los logs del nivel a los datos del bot
            os.chdir("..")
        dataset[bot_type].append(data) #añadimos los datos del bot a el dataset
        os.chdir("..")
    os.chdir("..")
    return dataset


def get_best_log(level_data_orig):
    level_data = level_data_orig.copy()
    for log in level_data:
        if not log["death"]:
            return log
    highest_points = {"index" : -1, "points" : -100000}
    i = 0
    for log in level_data:
        if log["points"] > highest_points["points"]:
            highest_points["index"] = i
            highest_points["points"] = log["points"]
        i += 1
    best_log = level_data[highest_points["index"]].copy()
    best_log["prior_deaths"] = 9 # no importa cual de las muertes fue la mejor, 
                                    #o cuantas muertes previas existen, solo importa que sea la mejor
    return best_log

def clean_data(dataset): # creamos un objeto con formato de dataframe conteniendo todos los datos
    cleaned_data = {"type" : [], "list" : []}
    i = 0
    # for lvl in dataset[list(dataset.keys())[0]][0]:
    #     data_sample = lvl[0]
    #     for data_key in data_sample.keys():
    #         if i != 0 or not (data_key == "health_lost_range" or data_key == "ranged_enem_killed"):
    #             if i != 1 or not (data_key == "health_lost_close" or data_key == "close_enem_killed"):
    #                 cleaned_data["level_" + str(i) + "_" + data_key] = []
    #     i += 1

    j = 0
    for key in dataset.keys():
        bot_data = {}
        for bot in dataset[key]:
            i = 0
            for level in bot:
                best_run = get_best_log(level)
                best_run["death"] = 1 if best_run["death"] else 0
                for log_key in best_run:
                    if i != 0 or not (log_key == "health_lost_range" or log_key == "ranged_enem_killed"):
                        if i != 1 or not (log_key == "health_lost_close" or log_key == "close_enem_killed"):
                            bot_data["level_" + str(i) + "_" + log_key] = best_run[log_key]
                i += 1
            #data[].append(bot_data)
            cleaned_data["list"].append(bot_data)
            cleaned_data["type"].append(key)
            j += 1
    return cleaned_data





def df_to_dataset(dataframe):
  dataframe = dataframe.copy()
  ds = tf.data.Dataset.from_tensor_slices((dict(dataframe), labels))
  return ds




original_dataset = load_data()
json_data = clean_data( original_dataset.copy())
#data = pd.DataFrame(json_data)
features = json_data['list']
labels = json_data['type']
model = tf.keras.models.load_model('saved_model/model')

print("0 = evader, 1 = health_prioritizer, 2 = random, 3 = ranged, 4 = speedrunner, 5 = tactic aggressive")
for i in range(len(features)):
    input_dict = {name: tf.convert_to_tensor([value]) for name, value in features[i].items()}
    val_pred =  model.predict(input_dict)
    # val_pred = tf.argmax(val_pred, 1, output_type=tf.dtypes.int64)
    prob = tf.nn.sigmoid(val_pred[0])
    print("\n")
    print(labels[i])
    print("evader: " + str(prob.numpy()[0]) + "%")
    print("health_prioritizer: " + str(prob.numpy()[1]) + "%")
    print("random: " + str(prob.numpy()[2]) + "%")
    print("tactic_aggressive: " + str(prob.numpy()[3]) + "%")
    print("speedrunner: " + str(prob.numpy()[4]) + "%")
    print("aggressive: " + str(prob.numpy()[5]) + "%")