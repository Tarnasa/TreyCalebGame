#	main.py
#	Use this file to start the game

import pygame, sys
from pygame.locals import *

from constants import *

from level import *

pygame.init()
fpsClock = pygame.time.Clock()

windowSurfaceObj = pygame.display.set_mode((GAME_WIDTH, GAME_HEIGHT))
pygame.display.set_caption(TITLE_MAIN)

ballImage = pygame.image.load('../res/circleRobot.png')
tileImage = pygame.image.load('../res/tileSingle.png')

map1 = Level(GAME_WIDTH / TILE_WIDTH, GAME_HEIGHT / TILE_HEIGHT)
map1block1 = TileObj()
map1block1.setImage(tileImage)
map1.data[0][0] = map1block1

while True:
	windowSurfaceObj.fill(BLACK)

	pygame.draw.line(windowSurfaceObj, WHITE, (16, 256), (512, 32), 4)
	pygame.draw.circle(windowSurfaceObj, WHITE, (300,15), 20, 0)

	map1.draw(windowSurfaceObj, 0, 0, GAME_WIDTH, GAME_HEIGHT)

	windowSurfaceObj.blit(ballImage, (0, 0))

	for event in pygame.event.get():
		if event.type == QUIT:
			pygame.quit()
			sys.exit()
		elif event.type == KEYDOWN:
			if event.key == K_ESCAPE:
				pygame.event.post(pgame.event.Event(QUIT))

	pygame.display.update()
	fpsClock.tick(60)


