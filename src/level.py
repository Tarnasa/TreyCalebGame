#	level.py
#	Stores tile data and entity data

from tileObj import *
from constants import *

class Level(object):
	def __init__(self, width, height):
		self.w = width
		self.h = height
		self.data = list()
		row = list()
		for x in range(self.w):
			row.append(TileObj())
		for y in range(self.h):
			self.data.append(row)

	def draw(self, surface, startx, starty, endx, endy):
		tileStartX = startx // TILE_WIDTH
		tileStartY = starty // TILE_HEIGHT
		tileEndX = endx // TILE_WIDTH + 1
		tileEndY = endy // TILE_HEIGHT + 1
		if (tileStartX < 0):
			tileStartX = 0
		if (tileStartY < 0):
			tileStartY = 0
		if (tileEndX >= self.w):
			tileEndX = self.w - 1
		if (tileEndY >= self.h):
			tileEndY = self.h - 1

		yo = -(starty % TILE_HEIGHT)
		for y in range(tileStartX, tileEndX):
			xo = -(startx % TILE_WIDTH)
			for x in range(tileStartY, tileEndY):
				print("x : " + str(x) + ", y : " + str(y))
				self.data[y][x].draw(surface, xo, yo)
				xo += TILE_WIDTH
			yo += TILE_HEIGHT

	def get(self, x, y):
		if (x >= 0 and x < self.w) and (y >= 0 and y < self.h):
			return self.data[y][x]
		else:
			return TileObj()
