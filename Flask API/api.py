from flask import Flask, jsonify, request
from flask_mysqldb import MySQL
import MySQLdb.cursors
from argon2 import PasswordHasher
from argon2.exceptions import VerifyMismatchError
import string
import secrets

# Not yet implemented
import requests

# Conservative settings for your VM
ph = PasswordHasher(
    time_cost=2,        # 2 iterations
    memory_cost=65536,  # 64 MB memory usage
    parallelism=2,      # Use both CPU cores
    hash_len=32,        # 32-byte hash output
    salt_len=16         # 16-byte salt
)

app = Flask(__name__)

# mail_url = "https://sandbox.api.mailtrap.io/api/send/3798191"

# mail_headers = {

#   "Authorization": "Bearer fc76494c18a490567b9e820d53fd67dc",

#   "Content-Type": "application/json"

# }

# MySQL config files
app.config['MYSQL_HOST'] = 'localhost'
app.config['MYSQL_USER'] = 'dungeonmaster'
app.config['MYSQL_PASSWORD'] = 'DungeonPass!@'
app.config['MYSQL_DB'] = 'dungeondaddies'
mysql = MySQL(app)

def check_account_exists(account_id, email, username):
    cursor = mysql.connection.cursor()
   
    # Build dynamic query based on provided parameters
    conditions = []
    params = []
    found_by = []
   
    if account_id is not None:
        conditions.append("id = %s")
        params.append(account_id)
        found_by.append("id")
   
    if email is not None:
        conditions.append("email = %s")
        params.append(email)
        found_by.append("email")
   
    if username is not None:
        conditions.append("username = %s")
        params.append(username)
        found_by.append("username")
   
    # If no parameters provided, return appropriate error status
    if not conditions:
        cursor.close()
        return {"status": "NO_PARAMS"}
   
    # Join conditions with OR (use AND if you want all conditions to match)
    query = "SELECT 1 FROM users WHERE " + " OR ".join(conditions)
   
    try:
        cursor.execute(query, tuple(params))
        result = cursor.fetchone()
        
        if result is not None:
            return {"status": "EXISTS", "found_by": found_by}
        else:
            return {"status": "NOT_EXISTS"}
    finally:
        cursor.close()


# Function for random password
def random_pw():
    min_amount_lowercase = 4
    min_amount_uppercase = 4
    min_amount_digits = 4
    max_pw_length = min_amount_lowercase + min_amount_uppercase + min_amount_digits + 3

    alphabet = string.ascii_letters + string.digits + '-_'
    while True:
        password = ''.join(secrets.choice(alphabet) for i in range(max_pw_length))
        if (sum(c.islower() for c in password) >= min_amount_lowercase
        and sum(c.isupper() for c in password) >= min_amount_uppercase
        and sum(c.isdigit() for c in password) >= min_amount_digits):
            return password


def r_pw():
    try:
        # data = request.get_json()
        # email = data.get('email')
        random_password = random_pw()

        # cursor = mysql.connection.cursor()

        password_hash = ph.hash(random_password)

        # query = """
        #     UPDATE TABLE users
        #     SET pw_hash = %s
        #     WHERE email = %s;
        # """

        # cursor.execute(query, (password_hash, email))
        # mysql.connection.commit()
        # cursor.close()

        mail_from = "noreply@dungeondaddies.com"
        mail_name = "Password reset"
        mail_to = "569383@student.fontys.nl"
        mail_subject = "Password reset"
        mail_text = f"Beste speler, \n\n Er is een wachtwoord reset op uw account aangevraagd. Uw nieuw wachtwoord is:\n {random_password}"

        payload = "{\"from\":{\"email\":\"" + mail_from + "\",\"name\":\"Mailtrap Test\"},\"to\":[{\"email\":\"" + mail_to + "\"}],\"subject\":\"" + mail_subject + "\",\"text\":\"" + mail_text + "\"}"
        
        response = requests.request("POST", mail_url, headers=mail_headers, data=payload)

        print(response.text)
        
    except Exception as e:
        print(f"Error fetching users: {e}")
        return jsonify({'success': False, 'message': 'Server error'}), 500
    

# Endpoint to register new user
@app.route('/register', methods=['POST'])
def create_user():
    try:
        # Assign data from POST request
        data = request.get_json()

        email = data.get('email')
        password = data.get('password')
        username = data.get('username')
        first_name = data.get('first_name')
        last_name = data.get('last_name')

        # Check if all fields are filled, return if not
        if not username or not email or not password or not first_name or not last_name:
            return jsonify({"error": "Missing required fields"}), 400

        # Check if email or username already exists, return if so
        account_exists = check_account_exists(account_id=None, email=email, username=username)

        if account_exists['status'] == "EXISTS":
            if account_exists['found_by'] is not None:
                return jsonify({"error": "Account already exists by" + account_exists['found_by']})
            
        # Hash password
        password_hash = ph.hash(password) 

        cursor = mysql.connection.cursor()

        # Query to insert user
        query = """
            INSERT INTO users (email, pw_hash, username, first_name, last_name) 
            VALUES (%s, %s, %s, %s, %s)
        """

        cursor.execute(query, (email, password_hash, username, first_name, last_name))

        mysql.connection.commit()
        cursor.close()

        return jsonify({'success': True, 'message': 'User created successfully'}), 201

    except Exception as e:
        print(f"Error creating user: {e}")
        return jsonify({'success': False, 'message': 'Server error'}), 500
    

# Login endpoint
@app.route('/login', methods=['POST'])
def login():
    try:
        # Assign data from POST request
        data = request.get_json()
        email = data.get('email')
        password = data.get('password')
        
        # Check if all fields are filled, return if not
        if not email or not password:
            return jsonify({'success': False, 'message': 'Missing required fields'}), 400
        
        cursor = mysql.connection.cursor()

        # Query to fetch user credentials
        query = """
            SELECT id, email, pw_hash 
            FROM users 
            WHERE email = %s
        """

        cursor.execute(query, (email,))
        user = cursor.fetchone()
        cursor.close()
        
        # Check if user exists
        if user:
            account_id, user_email, stored_hash = user
            
            # Verify entered password with stored (hashed) password
            try:
                ph.verify(stored_hash, password)
                return jsonify({
                    'success': True, 
                    'message': 'Login successful',
                    'account_id': account_id,
                    'email': user_email
                })

            except VerifyMismatchError:
                return jsonify({'success': False, 'message': 'Invalid credentials'}), 401
        
        else:
            return jsonify({'success': False, 'message': 'Invalid credentials'}), 401
            
    except Exception as e:
        print(f"Login error: {e}")
        return jsonify({'success': False, 'message': 'Server error'}), 500


# Reset password endpoint
@app.route('/reset-pw', methods=['POST'])
def reset_pw():
    try:
        # Get data from POST
        data = request.get_json()
        email = data.get('email')

        # Check if all fields are filled, return if not
        if not email:
            return jsonify({'success': False, 'message': 'Missing required fields'}), 400
        
        # Check if account exists, return if not
        account_exists = check_account_exists(account_id=None, email=email, username=None)
        if account_exists['status'] == "NOT_EXISTS":
            return jsonify({'success': False, 'message': 'Account not found'}), 404

        random_password = random_pw() # Generate random password
        password_hash = ph.hash(random_password) # Hash password

        cursor = mysql.connection.cursor()

        # Query to update old password with new password using user email
        query = """
            UPDATE users
            SET pw_hash = %s
            WHERE email = %s;
        """

        cursor.execute(query, (password_hash, email))
        mysql.connection.commit()
        
        cursor.close()
        
        # DEVELOPMENT PURPOSES ONLY!
        # This is a substitute for the email component, which is still WIP
        print(random_password)

        return jsonify({'success': True, 'message': 'Password reset successfully!'}), 201
        
    except Exception as e:
        print(f"Error updating password: {e}")
        return jsonify({'success': False, 'message': 'Server error'}), 500


# User list endpoint
@app.route('/fetch-users', methods=['GET'])
def fetch_users():
    try:
        cursor = mysql.connection.cursor(MySQLdb.cursors.DictCursor)

        # Query to fetch all users
        query = """
            SELECT u.email, u.username, u.first_name, u.last_name, u.created_at, u.updated_at, r.name as role_name
            FROM users u
            JOIN roles r ON u.role_id = r.id
        """
        cursor.execute(query)
            
        result = cursor.fetchall()
        cursor.close()
        
        return jsonify(result)

    except Exception as e:
        print(f"Error fetching users: {e}")
        return jsonify({'success': False, 'message': 'Server error'}), 500


# Edit user data endpoint
@app.route('/edit-user', methods=['POST'])
def edit_user():
    try:
        # Assign data from POST request
        data = request.get_json()
        account_id = data.get('account_id')
        email = data.get('email')
        username = data.get('username')
        first_name = data.get('first_name')
        last_name = data.get('last_name')
        role_id = data.get('role_id')

        # Check if all fields are filled, return if not
        if not account_id or not email or not username or not first_name or not last_name or not role_id:
            return jsonify({"error": "Missing required fields"}), 400
        
        cursor = mysql.connection.cursor()

        # Query to update user by given data
        query = """
            UPDATE users SET
            email = %s, username = %s, first_name = %s, last_name = %s, role_id = %s, updated_at = NOW()
            WHERE id = %s
        """

        cursor.execute(query, (email, username, first_name, last_name, role_id, account_id))

        mysql.connection.commit()
        cursor.close()
        
        return jsonify({'success': True, 'message': 'User updated successfully'}), 201

    except Exception as e:
        print(f"Error updating user: {e}")
        return jsonify({'success': False, 'message': 'Server error'}), 500


# Delete user endpoint
@app.route('/delete-user', methods=['POST'])
def delete_user():
    try:
        # Assing data from POST
        data = request.get_json()
        account_id = data.get('account_id')

        # Check if account exists, return if not
        account_exists = check_account_exists(account_id=account_id, email=None, username=None)

        if account_exists['status'] == "NO_PARAMS":
            return jsonify({"error": "No account id given"}), 400
        elif account_exists['status'] == "NOT_EXISTS":
            return jsonify({"error": "Account doesn't exist"}), 404

        cursor = mysql.connection.cursor()

        # Query to delete a user
        query = """
            DELETE FROM users
            WHERE id = %s
        """
        
        cursor.execute(query, (account_id,))

        mysql.connection.commit()
        cursor.close()
        
        return jsonify({'success': True, 'message': 'User deleted successfully'}), 201

    except Exception as e:
        print(f"Error deleting user: {e}")
        return jsonify({'success': False, 'message': 'Server error'}), 500


# Submit run endpoint
@app.route('/submit-run', methods=['POST'])
def submit_run():
    try:
        # Assign data from POST
        data = request.get_json()
        playtime = data.get('playtime')
        enemies_killed = data.get('enemies_killed')
        hits = data.get('hits')
        damage_dealt = data.get('damage_dealt')
        deaths = data.get('deaths')
        player_id = data.get('player_id')
        maps = data.get('maps', [])

        # Check if all fields are filled, return if not
        if not playtime or not enemies_killed or not hits or not damage_dealt or not deaths or not player_id or not maps:
            return jsonify({"error": "Missing required fields"}), 400

        # Check if account exists, return if not
        account_exists = check_account_exists(player_id, None, None)
        if account_exists['status'] == "NOT_EXISTS":
            return jsonify({"error": "Account not found"}), 404
        
        cursor = mysql.connection.cursor()

        # Query to get run number for player
        query_run_number = """
            SELECT COALESCE(MAX(player_run_number), 0) + 1 
            FROM runs 
            WHERE player_id = %s
        """

        cursor.execute(query_run_number, (player_id,))
        next_run_number = cursor.fetchone()[0]

        # Query to insert new run, using run fetched run number
        query_insert_run = """
            INSERT INTO runs (playtime, enemies_killed, hits, damage_dealt, deaths, player_id, player_run_number)
            VALUES (%s, %s, %s, %s, %s, %s, %s)
        """

        cursor.execute(query_insert_run, (playtime, enemies_killed, hits, damage_dealt, deaths, player_id, next_run_number))

        # Get last run id
        run_id = cursor.lastrowid

        # Insert map for each value in maps array
        for position, map_gen in enumerate(maps, start=1):
            query_insert_map = """
                INSERT INTO maps (map, run_id, run_position) 
                VALUES (%s, %s, %s)
            """
            cursor.execute(query_insert_map, (map_gen, run_id, position))

        mysql.connection.commit()
        cursor.close()

        return jsonify({'success': True, 'message': 'Run created successfully'}), 201

    except Exception as e:
        mysql.connection.rollback()
        print(f"Error creating run: {e}")
        return jsonify({'success': False, 'message': 'Server error'}), 500
    

# Fetch profile and stats
@app.route('/fetch-profile/<int:player_id>', methods=['GET'])
def fetch_profile(player_id):
    try:

        # Check if account exists, return if not
        account_exists = check_account_exists(account_id=player_id, email=None, username=None)

        if account_exists['status'] == "NO_PARAMS":
            return jsonify({"error": "Missing required fields"}), 400
        elif account_exists['status'] == "NOT_EXISTS":
            return jsonify({"error": "Account not found"}), 404

        cursor = mysql.connection.cursor(MySQLdb.cursors.DictCursor)

        # Query to fetch all player stats by using:
        # COUNT() to calculate total amount of runs
        # SUM() to calculate stats over all runs
        query = """
            SELECT 
            COUNT(*) AS total_runs,
            SUM(playtime) AS total_playtime, 
            SUM(enemies_killed) AS total_enemies_killed, 
            SUM(hits) AS total_hits, 
            SUM(damage_dealt) AS total_damage_dealt, 
            SUM(deaths) AS total_deaths
            FROM runs
            WHERE player_id = %s
        """

        cursor.execute(query, (player_id,))
        result = cursor.fetchall()
        cursor.close()

        return jsonify(result)

    except Exception as e:
        print(f"Error fetching user: {e}")
        return jsonify({'success': False, 'message': 'Server error'}), 500


# Fetch runs by user endpoint
@app.route('/fetch-runs/<int:account_id>', methods=['GET'])
def fetch_runs(account_id):
    try:
        
        # Check if account exists, return if not
        account_exists = check_account_exists(account_id=account_id, email=None, username=None)

        if account_exists['status'] == "NO_PARAMS":
            return jsonify({"error": "No account id given"}), 400
        elif account_exists['status'] == "NOT_EXISTS":
            return jsonify({"error": "Account doesn't exist"}), 404

        cursor = mysql.connection.cursor(MySQLdb.cursors.DictCursor)
        
        # Query to select all runs and username with player_id
        query = """
            SELECT r.date, r.playtime, r.deaths, r.player_run_number, u.username
            FROM runs r
            JOIN users u ON r.player_id = u.id
            WHERE r.player_id = %s;
        """

        cursor.execute(query, (account_id,))
        result = cursor.fetchall()

        cursor.close()

        return jsonify(result)
    
    except Exception as e:
        mysql.connection.rollback()
        print(f"Error fetching maps: {e}")
        return jsonify({'success': False, 'message': 'Server error'}), 500


if __name__ == '__main__':
    app.run(host="0.0.0.0", port=5000, debug=True)