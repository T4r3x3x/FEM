import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
from matplotlib import cm
import numpy as np
import sys

x = []
y = []
p = []
with open('pressure.txt', "r") as f:
    for line in f.readlines()[3:-1]:        
        arr = list(line.split(" "))
        arr = [elem.strip() for elem in arr]
        x.append(float(arr[0]))
        y.append(float(arr[1]))
        p.append(float(arr[2]))

x = np.asarray(x)
y = np.asarray(y)
p = np.asarray(p)
fig = plt.figure()
ax = fig.add_subplot(111, projection='3d')
ax.ticklabel_format(useOffset=False)
ax.plot_trisurf(x, y, p, linewidth=0, antialiased=False,
               label='pressure', shade=True, cmap=cm.inferno)
fig = plt.gcf()
fig.canvas.manager.set_window_title("pressure")
plt.show()