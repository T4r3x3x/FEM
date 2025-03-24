# -*- coding: cp1251 -*-
import sys
import matplotlib.pyplot as plt
from matplotlib.patches import Polygon


def get_elems_from_point(point, section2D):
    if section2D == "xy":
        return float(point[0]), float(point[1])
    if section2D == "xz":
        return float(point[0]), float(point[2])
    if section2D == "yz":
        return float(point[1]), float(point[2])
    else:
        raise Exception("Wrong section!")


args = sys.argv[1:]
sourceFilePath = args[0]
windowTitle = args[1]

colors = ["orange", "red", "green"]

fig, ax = plt.subplots(1)
fig.set_size_inches(12, 8)
fig.canvas.manager.set_window_title(windowTitle)

f = open(sourceFilePath, 'r', encoding="utf-8")

verticesXY = []
polygonsXY = []
subdomains = []
temp = []

ax.set_xlim(-10, 10)
ax.set_ylim(-10, 10)

elemCount = int(f.readline())

for line in range(0, elemCount):
    for number in range(0, 4):
        point = f.readline().split(' ')
        x, y = get_elems_from_point(point, 'xy')
        temp = [x, y]
        verticesXY.append(temp)
    f.readline()
    polygonsXY.append(Polygon(verticesXY, alpha=0.5, facecolor='none', edgecolor="red"))
    ax.add_patch(polygonsXY[line])
    verticesXY = []

ax.set_aspect('equal', adjustable='box')
plt.show()
