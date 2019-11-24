#!/usr/bin/python
# -*- coding: utf-8 -*-
JP = list()
CH = list()
USE = list()
EXCH = list()
EXJP = list()
Count = 0
with open("191124.txt",encoding='utf-8') as Lines:
    for Line in Lines:
        Line = Line.replace("\r\n","");
        Line = Line.replace("\r","");
        Line = Line.replace("\n","");
        if Count % 5 == 0:JP.append(Line)
        if Count % 5 == 1:CH.append(Line)
        if Count % 5 == 2:USE.append(Line)
        if Count % 5 == 3:EXCH.append(Line)
        if Count % 5 == 4:EXJP.append(Line)
        Count = Count + 1
for i in range(0,len(JP)):
    #print (JP[i] + "  <-->  " + USE[i])
    print (CH[i])
    
