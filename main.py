import noise
import numpy as np
from PIL import Image
import random, sys, json, os
from collections import deque

if len(sys.argv) <= 1:
    map_amount = 1
else:
    map_amount = int(sys.argv[1])

def make_map_int(arr):
    rows, cols = arr.shape
    for i in range(rows):
        for j in range(cols):
            arr[i, j] = int(arr[i, j])
    return arr

def set_map_numbers_from_random(arr, value):
    for i in range(len(arr)):
        for n in range(len(arr[0])):
            if arr[i][n] > 0:
                arr[i][n] = value
            else:
                arr[i][n] = 0
    return arr

def set_map_numbers(arr, old_value, new_value):
    for i in range(len(arr)):
        for n in range(len(arr[0])):
            if arr[i][n] == old_value:
                arr[i][n] = new_value
    return arr

#Functie die een noise map maakt doormiddel van perlin noise, dit wordt in een matrix gezet.
def generate_noise(size, seed):
    if seed == 0:
        random_seed = random.randint(0, 1000)
    else:
        random_seed = seed
    map = np.zeros(size)

    x_offset = random.uniform(0, 1000)
    y_offset = random.uniform(0, 1000)
    scale = random.uniform(20, 50)

    for i in range(size[0]):
        for j in range(size[1]):
            map[i][j] = noise.pnoise2(
                (i + x_offset) / scale,
                (j + y_offset) / scale,
                octaves=5,
                persistence=0.5,
                lacunarity=2.0,
                base=random_seed
            )
    return map

#Dit is een extra functie die de map neemt en er random noise overheen gooit.
def distort_map(map, intensity=random.uniform(0, 0.02)):
    noise_distort = np.random.uniform(-intensity, intensity, size=map.shape)
    return map + noise_distort

def has_valid_path(arr, start, end):
    rows, cols = arr.shape
    visited = [[False] * cols for _ in range(rows)]
    queue = deque([start])

    while queue:
        x, y = queue.popleft()

        if (x, y) == end:
            return True

        for dx, dy in [(-1,0), (1,0), (0,-1), (0,1)]:
            nx, ny = x + dx, y + dy
            if 0 <= nx < rows and 0 <= ny < cols:
                if arr[nx][ny] != 0 and not visited[nx][ny]:
                    visited[nx][ny] = True
                    queue.append((nx, ny))

    return False

#deze functie maakt een map en de ze nummers goed, lager dan 0 wordt 0 en hoger dan 0 wordt 1.
def generate_map(size, seed):
    map = generate_noise(size, seed)
    map = distort_map(map, intensity=0)
    map = set_map_numbers_from_random(map, 1)
    map = make_map_int(map)
    return map

#De laatste functie, deze kijkt doormiddel van BFS om te kijken of er een pad is door de map.
def generate_valid_map(size, seed):
    map_shape = size

    map_start = (int(map_shape[0] / 2), 0)
    map_end = (int(map_shape[0] / 2), map_shape[1] - 1)

    valid_map_found = False
    map_counter = 1
    while not valid_map_found:
        #print("Currently chacking map: " + str(map_counter))
        map = generate_map(map_shape, seed)
        valid_map_found = has_valid_path(map, map_start, map_end)
        map_counter += 1
    #print("found a valid map on the: " + str(map_counter - 1) + "th try")
    return map



def add_random_in_list(arr, value, target, amount):
    positions = []
    shape = arr.shape

    for i in range(shape[0]):
        for j in range(shape[1]):
            if arr[i, j] == value:  
                positions.append((i, j))

    selected_indices = random.sample(positions, k=amount)

    for i, j in selected_indices:   
        arr[i, j] = target

    return arr

def gen_map(map_amount):
    os.makedirs("maps", exist_ok=True)
    maps = []
    for i in range(map_amount):
        map = generate_valid_map((51, 51), 0)
        map = add_random_in_list(map, 1, 2, 10)
        map = add_random_in_list(map, 1, 3, 10)
        print(map)
        # for i in range(len(map)):
        #     print(map[i])
        with open('maps/data'+str(i)+'.json', 'w', encoding='utf-8') as f:
            json.dump(map.tolist(), f, ensure_ascii=False, indent=4)

print(gen_map(map_amount))