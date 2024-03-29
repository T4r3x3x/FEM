import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
from matplotlib import cm
import numpy as np
import sys

X = []
y = []
p = []
with open('output\pressure.txt', "r") as f:
    for line in f.readlines()[3:-1]:        
        arr = list(line.split(" "))
        arr = [elem.strip() for elem in arr]
        X.append(float(arr[0]))
        y.append(float(arr[1]))
        p.append(float(arr[2]))

X = np.asarray(X)
y = np.asarray(y)
p = np.asarray(p)
fig = plt.figure(figsize=(27,18))
ax = fig.add_subplot(111, projection='3d')
ax.ticklabel_format(useOffset=False)
ax.plot_trisurf(X, y, p, linewidth=0, antialiased=False,
               label='pressure', shade=True, cmap=cm.inferno)
fig = plt.gcf()
fig.canvas.manager.set_window_title("pressure")
plt.savefig('Images\Pressure.png', bbox_inches='tight') 
fig.set_size_inches(12,9)
plt.show()