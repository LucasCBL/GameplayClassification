import os
import numpy as np
import matplotlib.pyplot as plt
import json
import pandas as pd
import seaborn as sns


def load_data():
    dataset = {}
    os.chdir("DATA")
    # cambiamos la carpeta de trabajo

    # lista de carpetas con datos, estas se llaman con la id del bot
    folders = os.listdir()
    for bot_data in folders:
        split_name = bot_data.split("_")
        #print(split_name)
        bot = split_name.index("bot")
        bot_type = "_"
        bot_type = bot_type.join(split_name[:bot])
        if(bot_type == "tactic_aggressive"):
            bot_type = "aggressive"
        
        if(bot_type == "ranged_attacker"):
            bot_type = "tactic_aggressive"
        
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

# def clean_data(savefile, dataset):
#     data = [[], []]
#     for key in dataset.keys():
#         for bot in dataset[key]:
#             bot_data = []
#             for level in bot:
#                 best_run = get_best_log(level)
#                 best_run["death"] = 1 if best_run["death"] else 0
#                 for log_key in best_run:
#                     bot_data.append(best_run[log_key])
#             data[0].append(bot_data)
#             data[1].append(key)
#     with open(savefile + ".json", 'w') as outfile:
#         json.dump(data, outfile)
        
def clean_data(savefile, dataset): # creamos un objeto con formato de dataframe conteniendo todos los datos
    cleaned_data = {"type" : []}
    i = 0
    for lvl in dataset[list(dataset.keys())[0]][0]:
        data_sample = lvl[0]
        for data_key in data_sample.keys():
            if i != 0 or not (data_key == "health_lost_range" or data_key == "ranged_enem_killed"):
                if i != 1 or not (data_key == "health_lost_close" or data_key == "close_enem_killed"):
                    cleaned_data["level_" + str(i) + "_" + data_key] = []
        i += 1

    j = 0
    for key in dataset.keys():
        for bot in dataset[key]:
            i = 0
            for level in bot:
                best_run = get_best_log(level)
                best_run["death"] = 1 if best_run["death"] else 0
                for log_key in best_run:
                    if i != 0 or not (log_key == "health_lost_range" or log_key == "ranged_enem_killed"):
                        if i != 1 or not (log_key == "health_lost_close" or log_key == "close_enem_killed"):
                            cleaned_data["level_" + str(i) + "_" + log_key].append(best_run[log_key])
                i += 1
            #data[].append(bot_data)
            cleaned_data["type"].append(key)
            j += 1

    with open(savefile + ".json", 'w') as outfile:
        json.dump(cleaned_data, outfile)

def get_average_data(dataset):
    average_data = {}
    for key in dataset.keys():
        average_data[key] = []
        total = '' # objeto que contiene la suma de los mejores datos de cada run en cada nivel
        for bot in dataset[key]:
            if not total:
                total = [] # creamos el array de niveles
                # si no existe el objeto total le asignamos los valores del primer bot como total actual
                for level in bot:
                    best_run = get_best_log(level)
                    best_run["death"] = 1 if best_run["death"] else 0
                    total.append(best_run)
            else:
                i = 0
                # sumamos los mejores datos del bot actual a el total
                for level in bot:
                    best_run = get_best_log(level)
                    best_run["death"] = 1 if best_run["death"] else 0
                    for data_key in best_run.keys():
                        total[i][data_key] += best_run[data_key]
                    i += 1
        for level in total:
            average = level.copy()
            for data_key in level.keys():
                # sacamos la media de cada valor
                average[data_key] = level[data_key] / len(dataset[key])
            average_data[key].append(average)
    return average_data



original_dataset = load_data()
clean_data("datos", original_dataset.copy())

with open("datos.json") as infile:
    dataframe = pd.DataFrame(json.loads(infile.read()))

df_keys = dataframe.keys()
 
for data_type in df_keys:
    if(not data_type == "type"):
        sns.distplot(dataframe.loc[dataframe["type"] == "aggressive"][data_type], label = "aggressive")

        sns.distplot(dataframe.loc[dataframe["type"] == "tactic_aggressive"][data_type], label = "tactic_aggressive")

        sns.distplot(dataframe.loc[dataframe["type"] == "enemy_evader"][data_type], label = "enemy_evader")

        sns.distplot(dataframe.loc[dataframe["type"] == "health_prioritizer"][data_type], label = "health prioritizer")

        sns.distplot(dataframe.loc[dataframe["type"] == "random"][data_type], label = "random")

        sns.distplot(dataframe.loc[dataframe["type"] == "speedrunner"][data_type], label = "speedrunner")

        plt.legend()
        plt.show()
print("clases de bots: " + str(list(original_dataset.keys())))
print("cantidad de datos por clase de bot")
for key in original_dataset.keys():
    print("\t -" + key + ": " + str(len(original_dataset[key])))

avg_data = get_average_data(original_dataset.copy())

print("MEDIAS DE CADA TIPO DE BOT SEGUN EL NIVEL")
for key in avg_data.keys():
    print("\t " + key + ":")
    i = 0
    for level in avg_data[key]:
        print("\t\t-Nivel " + str(i) + ": ")
        for data_key in level.keys():
            print("\t\t\t•" + data_key + ": " + str(level[data_key]))
        i += 1

print("MEDIAS DE CADA TIPO DE BOT SEGUN EL NIVEL VISUALIZADAS")
for data_key in avg_data[list(avg_data.keys())[0]][0]:
    plt.figure()
    for key in avg_data.keys():
        data_array = []
        for level in avg_data[key]:
            data_array.append(level[data_key])
        plt.plot(data_array, label=key)
    plt.title("mean " + data_key)
    plt.legend()
    

plt.show()
        

    

