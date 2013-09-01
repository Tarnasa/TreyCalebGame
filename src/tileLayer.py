#
#
#

import pygame
from pygame.locals import *

from tileObj import *
from constants import *

class TileLayer(object):
	def __init__(self, depth, levelWidth, levelHeight):
		self.data = list()
		self.depth = depth
		self.w = levelWidth
		self.h = levelHeight
		self.xChunks = self.w // CHUNK_WIDTH + 1
		self.yChunks = self.h // CHUNK_HEIGHT + 1
		self.chunks = list()
		for i in range(self.xChunks * self.yChunks):
			self.chunks.append(list())

	def addTile(self, tileObj):
		chunkX = tileObj.x // CHUNK_WIDTH
		chunkY = tileObj.y // CHUNK_HEIGHT
		if not self.validChunk(chunkX, chunkY):
			return False
		self.chunks[chunkX + chunkY * self.xChunks].append(tileObj)

	def draw(self, surface, viewX, viewY, viewW, viewH):
		chunkSX = viewX // CHUNK_WIDTH
		chunkSY = viewY // CHUNK_HEIGHT
		chunkEX = (viewX + viewW) // CHUNK_WIDTH + 1
		chunkEY = (viewY + viewH) // CHUNK_HEIGHT + 1
		for y in range(chunkSY, chunkEY):
			i = y * self.xChunks + chunkSX
			for x in range(chunkSX, chunkEX):
				for tile in self.chunks[i]:
					if tile.visible:
						surface.blit(tile.image, (tile.x - viewX, tile.y - viewY))

	def findTileAt(self, x, y):
		chunkX = x // CHUNK_WIDTH
		chunkY = y // CHUNK_HEIGHT
		if not self.validChunk(chunkX, chunkY):
			return False
		for tile in self.chunks[chunkX + chunkY * self.xChunks]:
			if tile.x == x and tile.y == y:
				return tile
		return False

	def collideWithTileAt(self, x, y):
		chunkX = x // CHUNK_WIDTH
		chunkY = y // CHUNK_HEIGHT
		if not self.validChunk(chunkX, chunkY):
			return False
		for tile in self.chunks[chunkX + chunkY * self.xChunks]:
			if tile.solid:
				if x >= tile.x and y >= tile.y and x < tile.x + tile.w and y < tile.y + tile.h:
					return tile
		return False

	def validChunk(self, x, y):
		return x < self.xChunks and y < self.yChunks and x >= 0 and y >= 0