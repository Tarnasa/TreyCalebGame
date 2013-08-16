#	level.py
#	Stores tile data and entity data

from tileObj import *

class Level(object):
	def __init__(self, width, height):
		self.w = width
		self.h = height
		self.data = list()
		row = list()
		for y in range(self.h):
			row += [TileObj()]
		self.data += [row]

	#	Expects tileObj
	def fillGrid(self, value):
		for y in range(self.h):
			for x in range(self.w):
				data[x][y] = value
