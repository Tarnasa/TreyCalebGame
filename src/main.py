#	main.py
#	Use this file to start the game

import pygame, sys
from pygame.locals import *

from constants import *

pygame.init()
fpsClock = pygame.time.Clock()

windowSurfaceObj = pygame.display.set_mode((800, 600))
pygame.display.set_caption(TITLE_MAIN)

while True:
	windowSurfaceObj.fill(BLACK)

	pygame.draw.line(windowSurfaceObj, WHITE, (16, 256), (512, 32), 4)

	for event in pygame.event.get():
		if event.type == QUIT:
			pygame.quit()
			sys.exit()
		elif event.type == KEYDOWN:
			if event.key == K_ESCAPE:
				pygame.event.post(pgame.event.Event(QUIT))

	pygame.display.update()
	fpsClock.tick(60)


