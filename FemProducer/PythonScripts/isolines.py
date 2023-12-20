from tempfile import tempdir
from turtle import xcor
import numpy as np
import matplotlib.pyplot as plt
import matplotlib.tri as tri 
from matplotlib.patches import Polygon

def YLine(x,  x1,  y1,  x2,  y2):

    if y1 == y2:
        return y1

    if y1 > y2:
        temp = x1  
        x1 = x2
        x2 = temp
      
        temp = y1  
        y1 = y2
        y2 = temp
        
    y = (x - x1) * (y2 - y1) / (x2 - x1) + y1
    return y


def isPointInside(subdomain, point):
     
    if subdomain[0] <= point[0] and point[0] <= subdomain[2]:
        if YLine(point[0],subdomain[0],subdomain[1],subdomain[2],subdomain[3]) <= point[1] and point[1] <= YLine(point[0],subdomain[4],subdomain[5],subdomain[6],subdomain[7]):
            return True
        
    return False
        

def getMaskz(xmid, ymid, subdomains):    
    mask = []
    for i in range(len(xmid)):
        flag = True
        point = [xmid[i], ymid[i]]   
        for j in range(len(subdomains)):
            if isPointInside(np.array(subdomains[j]), np.array(point)):       
                flag = False
                break
        mask.append(flag)
    return mask

fig, ax = plt.subplots()

# Define the vertices for the first polygon
f = open('C:\\Users\\hardb\\source\\repos\\FEM\\FemProducer\\bin\\Debug\\net8.0\\isolines.txt', 'r', encoding="utf-8")
#elemCount = int(f.readline())
x = []
y = []
z = []
mask = []
temp = []
subdomains = []
subdomainsCount = int(f.readline())

for i in range(0,subdomainsCount):
	point = f.readline().split(' ')
	for j in range(0,8):
		temp.append(float(point[j]))
	subdomains.append(temp)
	temp =[]

nodeCount = int(f.readline())
for line in range(0,nodeCount):
	point = f.readline().split(' ')
	x.append(float(point[0]))
	y.append(float(point[1]))		
	z.append(float(point[2]))
	

# Z_masked = np.ma.masked_array(z, mask)

triang = tri.Triangulation(x, y) 

ax.set_xlabel('X-axis')
ax.set_ylabel('Y-axis')
xmid = np.array(x)[triang.triangles].mean(axis=1)
ymid = np.array(y)[triang.triangles].mean(axis=1)

mask = getMaskz(xmid,ymid,subdomains)

triang.set_mask(mask)
# _x = sorted(list(set(x)))
# _y = sorted(list(set(x)))
plt.tricontourf(triang, z)
plt.colorbar()
plt.show()
