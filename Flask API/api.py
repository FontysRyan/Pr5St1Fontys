from flask import Flask, jsonify
# from flask_mysqldb import MySQL
# from sqlalchemy import create_engine, text, MetaData

app = Flask(__name__)
# MySQL configuration
# app.config['MYSQL_HOST'] = 'localhost'
# app.config['MYSQL_USER'] = 'dungeonmaster'
# app.config['MYSQL_PASSWORD'] = 'DungeonPass!@'
# app.config['MYSQL_DB'] = 'dungeondaddies'
# mysql = MySQL(app)

@app.route('/')
def hello_world():
    return 'Hello, World!'

if __name__ == '__main__':
    app.run(host="0.0.0.0", port=5000, debug=True)