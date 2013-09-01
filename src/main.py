#	main.py
#	Use this file to start the game

import pygame, sys
from pygame.locals import *

from constants import *
from view import *
from room import *
from tileObj import *

pygame.init()
fpsClock = pygame.time.Clock()

windowSurfaceObj = pygame.display.set_mode((GAME_WIDTH, GAME_HEIGHT))
pygame.display.set_caption(TITLE_MAIN)

ballImage = pygame.image.load('../res/circleRobot.png')
tileImage = pygame.image.load('../res/tileSingle.png')

room1 = Room(1024, 1024)

mainView = View(GAME_WIDTH, GAME_HEIGHT, room1.w, room1.h)

room1.addTile(0, TileObj(64, 32).setImage(tileImage))

while True:
	windowSurfaceObj.fill(BLACK)

	room1.draw(windowSurfaceObj, mainView)

	#pygame.draw.line(windowSurfaceObj, WHITE, (16, 256), (512, 32), 4)
	#pygame.draw.circle(windowSurfaceObj, WHITE, (300,15), 20, 0)

	windowSurfaceObj.blit(ballImage, (128, 64))

	for event in pygame.event.get():
		if event.type == QUIT:
			pygame.quit()
			sys.exit()
		elif event.type == KEYDOWN:
			if event.key == K_ESCAPE:
				pygame.event.post(pgame.event.Event(QUIT))

	pygame.display.update()
	fpsClock.tick(60)


