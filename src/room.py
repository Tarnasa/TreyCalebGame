#	room.py
#

from tileLayer import *

def getTileLayerDepth(tileLayer):
	return tileLayer.depth

class Room(object):
	def __init__(self, width, height):
		self.w = width
		self.h = height
		self.tileLayers = list()
		self.tiles = dict()

	def addTile(self, depth, tile):
		for tileLayer in self.tileLayers:
			if tileLayer.depth == depth:
				if tileLayer.addTile(tile):
					self.tiles.append(str(tile.x) + ',' + str(tile.y), tile)
					return True
				else:
					return False
		#[OPTIMIZE]
		newTileLayer = TileLayer(depth, self.w, self.h)
		self.tileLayers.append(newTileLayer)
		self.tileLayers = sorted(self.tileLayers, key=getTileLayerDepth)
		if newTileLayer.addTile(tile):
			self.tiles.append(str(tile.x) + ',' + str(tile.y), tile)
			return True
		else:
			return False

	def draw(self, surface, view):
		for tileLayer in self.tileLayers:
			tileLayer.draw(surface, view.x, view.y, view.w, view.h)

		
		