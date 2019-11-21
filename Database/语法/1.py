#!/usr/bin/python
# -*- coding: utf-8 -*-
JP = list()
CH = list()
EXJP = list()
EXCH = list()
Count = 0
with open("JPCH.txt",encoding='utf-8') as Lines:
    for Line in Lines:
        if Count % 5 == 0:JP.append(Line)
        if Count % 5 == 1:CH.append(Line)
        if Count % 5 == 2:EXJP.append(Line)
        if Count % 5 == 3:EXCH.append(Line)
        Count = Count + 1
for Line in EXCH:
    print(Line,end="")
            
