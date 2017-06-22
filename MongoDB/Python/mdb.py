from flask import Flask
from pymongo import MongoClient
import datetime

app = Flask(__name__)

@app.route('/insert')
def insert():
    client = MongoClient('localhost', 27017)
    db = client['test']
    collection = db['people']
    person = {"name": "Cristina", "age": 27, "car": "Audi", "country": "Spain", "email": "cristina@hotmail.com", "timestamp": datetime.datetime.now()}
    people = [{"name": "Pamela", "age": 44, "car": "Audi", "country": "EEUU", "email": "pamela@hotmail.com", "timestamp": datetime.datetime.now()}, 
              {"name": "Katy", "age": 24, "car": "Mazda", "country": "UK", "email": "katy@hotmail.com", "timestamp": datetime.datetime.now()}, 
              {"name": "Eva", "age": 37, "car": "Renault", "country": "Italy", "email": "eva@hotmail.com", "timestamp": datetime.datetime.now()}]
    collection.insert_one(person)
    collection.insert_many(people)
    client.close()

    return 'Hello, World!'

@app.route('/find')
def find():
    client = MongoClient('localhost', 27017)
    db = client['test']
    collection = db['people']
    cursor = collection.find({"car": "Audi"})
    for document in cursor:
        print document["name"]
    client.close()
    return ""

@app.route('/update')
def update():
    client = MongoClient('localhost', 27017)
    db = client['test']
    collection = db['people']
    collection.update_one({"name": "Eva"}, {"$set": {"car": "Peugeot"}, "$currentDate": {"timestamp": True}})
    client.close()
    return ""

@app.route('/remove')
def remove():
    client = MongoClient('localhost', 27017)
    db = client['test']
    collection = db['people']
    collection.delete_one({"country": "UK"})
    client.close()

if __name__ == '__main__':
    app.run()