#!/usr/bin/env python
# -*- coding: iso-8859-15 -*-

# Description: File downloader library for SkyPIC
# Copyright (C) 2007 by Rafael Treviño Menéndez
# Author: Rafael Treviño Menéndez <skasi.7@gmail.com>
#         Juan Gonzalez <juan@iearobotics.com>

# This program is free software; you can redistribute it and/or
# modify it under the terms of the GNU Library General Public
# License as published by the Free Software Foundation; either
# version 2 of the License, or (at your option) any later version.

# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
# Library General Public License for more details.

# You should have received a copy of the GNU General Public License
# along with this program; if not, write to the Free Software
# Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.


#----------------------------------------------------------------------------
"""
 Libreria que contiene FIRMWARE para la Skypic 
 El firmware ha sido obtenido a partir de los ficheros .hex utilizando 
 la herramienta hex2python, de la siguiente manera:

  $ ./hex2python.py fichero.hex f3 nombre_lista

 Por ejemplo, el clasico programa del led parpdeante se ha obtenido asi:

 ./hex2python.py ledp1.hex f3 ledp1
"""
#----------------------------------------------------------------------------


#---------------------------------------------------------------------------
#- LEDON. Encender el led de la Skypic
#---------------------------------------------------------------------------
ledon=[[0x0000, 0x0000, 0x118A, 0x120A, 0x280C, 0x0000, 0x1683, 0x1303, 0x1086, 0x1283, 0x1486, 0x280A, 0x0008, 0x118A, 0x120A, 0x2805],]

#----------------------------------------------------------------------
#- LEDP1.  Programa del ledp, que hace parpadear el led de la skypic
#----------------------------------------------------------------------
ledp1=[[0x0000, 0x0000, 0x118A, 0x120A, 0x2821, 0x0000, 0x30FD, 0x1683, 0x1303, 0x0086, 0x3002, 0x1283, 0x0686, 0x30FF, 0x00FF, 0x30FF, 0x2012],[0x0010, 0x2809, 0x0008, 0x1283, 0x1303, 0x00A3, 0x087F, 0x00A2, 0x0822, 0x0423, 0x1903, 0x2820, 0x30FF, 0x07A2, 0x1C03, 0x03A3, 0x2817],[0x0020, 0x0008, 0x118A, 0x120A, 0x2805],]

#----------------------------------------------------------------------
#- LEDP2.  Programa del ledp, que hace parpadear el led de la skypic. 
#- Se diferencia del ledp1 en que el parpadeo es mas rapido
#----------------------------------------------------------------------
ledp2=[[0x0000, 0x0000, 0x118A, 0x120A, 0x2821, 0x0000, 0x30FD, 0x1683, 0x1303, 0x0086, 0x3002, 0x1283, 0x0686, 0x3000, 0x00FF, 0x3080, 0x2012],[0x0010, 0x2809, 0x0008, 0x1283, 0x1303, 0x00A3, 0x087F, 0x00A2, 0x0822, 0x0423, 0x1903, 0x2820, 0x30FF, 0x07A2, 0x1C03, 0x03A3, 0x2817],[0x0020, 0x0008, 0x118A, 0x120A, 0x2805],]

#----------------------------------------------------------------------------
#-          FIRMWARE DEL PROYECTO STARGATE 
#----------------------------------------------------------------------------

#--- Servidor de Eco. V1. Velocidad 9600 Baudios
echo=[[0x0000, 0x0183, 0x3000, 0x008A, 0x2804, 0x1683, 0x3081, 0x0099, 0x3024, 0x0098, 0x1283, 0x3090, 0x0098, 0x1683, 0x0186, 0x1283, 0x30FF],[0x0010, 0x0086, 0x2015, 0x2019, 0x0086, 0x2811, 0x1E8C, 0x2815, 0x081A, 0x0008, 0x1E0C, 0x2819, 0x0099, 0x0008],]

#--- Servidor GENERICO. V1. Velocidad 9600 baudios
generic=[[0x0000, 0x0183, 0x3000, 0x008A, 0x2804, 0x1683, 0x3081, 0x0099, 0x3024, 0x0098, 0x1283, 0x3090, 0x0098, 0x1683, 0x1086, 0x1283, 0x3002],[0x0010, 0x0086, 0x2048, 0x00A0, 0x3050, 0x0220, 0x1903, 0x2824, 0x3049, 0x0220, 0x1903, 0x2827, 0x304C, 0x0220, 0x1903, 0x2830, 0x3053],[0x0020, 0x0220, 0x1903, 0x2838, 0x2811, 0x304F, 0x204C, 0x2811, 0x3049, 0x204C, 0x3020, 0x204C, 0x3030, 0x204C, 0x3010, 0x204C, 0x2811],[0x0030, 0x203E, 0x0800, 0x00A3, 0x304C, 0x204C, 0x0823, 0x204C, 0x2811, 0x203E, 0x2048, 0x0080, 0x3053, 0x204C, 0x2811, 0x2048, 0x0084],[0x0040, 0x2048, 0x00A1, 0x1821, 0x2846, 0x1383, 0x2847, 0x1783, 0x0008, 0x1E8C, 0x2848, 0x081A, 0x0008, 0x1E0C, 0x284C, 0x0099, 0x0008],]

#--- Servidor SERVOS8. V1. Velocidad 9600 baudios
servos8=[[0x0000, 0x0000, 0x118A, 0x120A, 0x2907, 0x00F2, 0x0E03, 0x0183, 0x1283, 0x1303, 0x00F1, 0x080A, 0x00F0, 0x018A, 0x110B, 0x3000, 0x1283],[0x0010, 0x1303, 0x042B, 0x1D03, 0x2820, 0x0829, 0x052A, 0x1283, 0x1303, 0x0086, 0x30EA, 0x0081, 0x3001, 0x1283, 0x1303, 0x00AB, 0x2862],[0x0020, 0x082B, 0x3A01, 0x1D03, 0x2839, 0x0828, 0x3E20, 0x00AF, 0x3000, 0x1803, 0x3E01, 0x00B0, 0x082F, 0x0084, 0x1383, 0x1830, 0x1783],[0x0030, 0x0800, 0x1283, 0x1303, 0x0081, 0x3002, 0x1283, 0x1303, 0x00AB, 0x2862, 0x082B, 0x3A02, 0x1D03, 0x2861, 0x1283, 0x1303, 0x0186],[0x0040, 0x1283, 0x1303, 0x0828, 0x3E20, 0x00AF, 0x3000, 0x1803, 0x3E01, 0x00B0, 0x082F, 0x0084, 0x1383, 0x1830, 0x1783, 0x0800, 0x3C52],[0x0050, 0x1283, 0x1303, 0x0081, 0x1283, 0x1303, 0x01AB, 0x1003, 0x0DA9, 0x0AA8, 0x3000, 0x0429, 0x1D03, 0x2862, 0x3001, 0x00A9, 0x01A8],[0x0060, 0x2862, 0x01AB, 0x1283, 0x1303, 0x0870, 0x008A, 0x0183, 0x0E71, 0x0083, 0x0EF2, 0x0E72, 0x0009, 0x1683, 0x1303, 0x0186, 0x3002],[0x0070, 0x1283, 0x0086, 0x1283, 0x1303, 0x01B4, 0x3008, 0x0234, 0x1803, 0x2889, 0x0834, 0x3E20, 0x00B5, 0x3000, 0x1803, 0x3E01, 0x00B6],[0x0080, 0x0835, 0x0084, 0x1383, 0x1836, 0x1783, 0x30B0, 0x0080, 0x0AB4, 0x2875, 0x01AA, 0x01A8, 0x01AB, 0x3001, 0x00A9, 0x20FD, 0x20DF],[0x0090, 0x20F7, 0x1283, 0x1303, 0x00B4, 0x3A45, 0x1903, 0x28AA, 0x0834, 0x3A49, 0x1903, 0x28A6, 0x0834, 0x3A50, 0x1903, 0x28A4, 0x0834],[0x00A0, 0x3A57, 0x1903, 0x28A8, 0x2890, 0x20DC, 0x2890, 0x20D3, 0x2890, 0x20B2, 0x2890, 0x20AD, 0x2890, 0x0008, 0x20F7, 0x1283, 0x1303],[0x00B0, 0x00AA, 0x0008, 0x20F7, 0x1283, 0x1303, 0x00AC, 0x20F7, 0x1283, 0x1303, 0x00B1, 0x3099, 0x0231, 0x1C03, 0x28C0, 0x3099, 0x00B1],[0x00C0, 0x032C, 0x00B2, 0x3E20, 0x00B2, 0x3000, 0x1803, 0x3E01, 0x00B3, 0x0831, 0x3CFF, 0x00B1, 0x0832, 0x0084, 0x1383, 0x1833, 0x1783],[0x00D0, 0x0831, 0x0080, 0x0008, 0x3049, 0x20E9, 0x3030, 0x20E9, 0x3030, 0x20E9, 0x3011, 0x20E9, 0x0008, 0x304F, 0x20E9, 0x0008, 0x3005],[0x00E0, 0x1683, 0x1303, 0x0081, 0x110B, 0x168B, 0x178B, 0x1283, 0x0181, 0x0008, 0x1283, 0x1303, 0x00AE, 0x1283, 0x1303, 0x1E0C, 0x28EC],[0x00F0, 0x1283, 0x1303, 0x082E, 0x1283, 0x1303, 0x0099, 0x0008, 0x1283, 0x1303, 0x1E8C, 0x28F7, 0x081A, 0x0008, 0x3081, 0x1683, 0x1303],[0x0100, 0x0099, 0x3024, 0x0098, 0x3090, 0x1283, 0x0098, 0x0008, 0x118A, 0x120A, 0x286C],]


#--- Servidor PICP. V2. Velocidad 9600 baudios
picp=[[0x0000, 0x0000, 0x118A, 0x120A, 0x2997, 0x0000, 0x218D, 0x3067, 0x1683, 0x0086, 0x1283, 0x0186, 0x2174, 0x2187, 0x1283, 0x1303, 0x00B5],[0x0010, 0x3A41, 0x1903, 0x2842, 0x0835, 0x3A42, 0x1903, 0x2840, 0x0835, 0x3A43, 0x1903, 0x2846, 0x0835, 0x3A44, 0x1903, 0x283E, 0x0835],[0x0020, 0x3A49, 0x1903, 0x283A, 0x0835, 0x3A4A, 0x1903, 0x284A, 0x0835, 0x3A50, 0x1903, 0x2838, 0x0835, 0x3A52, 0x1903, 0x2844, 0x0835],[0x0030, 0x3A54, 0x1903, 0x283C, 0x0835, 0x3A57, 0x1903, 0x2848, 0x280C, 0x20E2, 0x280C, 0x20D9, 0x280C, 0x20CD, 0x280C, 0x20BA, 0x280C],[0x0040, 0x20B5, 0x280C, 0x20B0, 0x280C, 0x20A2, 0x280C, 0x2099, 0x280C, 0x2084, 0x280C, 0x204D, 0x280C, 0x0008, 0x2187, 0x1283, 0x1303],[0x0050, 0x00B2, 0x2187, 0x1283, 0x1303, 0x00B3, 0x3000, 0x0433, 0x1903, 0x285C, 0x3001, 0x00B4, 0x285D, 0x01B4, 0x3000, 0x0434, 0x1903],[0x0060, 0x286F, 0x30FF, 0x00B4, 0x3006, 0x2110, 0x1283, 0x1303, 0x0BB4, 0x2863, 0x3006, 0x2110, 0x1283, 0x1303, 0x03B3, 0x2855, 0x3000],[0x0070, 0x0432, 0x1903, 0x2876, 0x3001, 0x00B3, 0x2877, 0x01B3, 0x3000, 0x0433, 0x1903, 0x2881, 0x3006, 0x2110, 0x1283, 0x1303, 0x03B2],[0x0080, 0x286F, 0x304A, 0x2179, 0x0008, 0x2187, 0x1283, 0x1303, 0x00B0, 0x2187, 0x1283, 0x1303, 0x00B1, 0x3002, 0x2110, 0x1283, 0x1303],[0x0090, 0x0830, 0x00FF, 0x0831, 0x20FA, 0x3008, 0x2110, 0x3057, 0x2179, 0x0008, 0x3000, 0x2110, 0x3000, 0x00FF, 0x3000, 0x20FA, 0x3043],[0x00A0, 0x2179, 0x0008, 0x3004, 0x2110, 0x20E5, 0x3052, 0x2179, 0x1283, 0x1303, 0x0820, 0x2179, 0x1283, 0x1303, 0x0821, 0x2179, 0x0008],[0x00B0, 0x3006, 0x2110, 0x3041, 0x2179, 0x0008, 0x3008, 0x2110, 0x3042, 0x2179, 0x0008, 0x2187, 0x1283, 0x1303, 0x00AE, 0x2187, 0x1283],[0x00C0, 0x1303, 0x00AF, 0x3002, 0x2110, 0x1283, 0x1303, 0x082E, 0x00FF, 0x082F, 0x20FA, 0x3044, 0x2179, 0x0008, 0x3010, 0x1283, 0x1303],[0x00D0, 0x0086, 0x3002, 0x2161, 0x1283, 0x1303, 0x0186, 0x3054, 0x2179, 0x0008, 0x3049, 0x2179, 0x3040, 0x2179, 0x3030, 0x2179, 0x3012],[0x00E0, 0x2179, 0x0008, 0x304F, 0x2179, 0x0008, 0x1683, 0x1303, 0x1786, 0x3001, 0x2127, 0x3008, 0x2127, 0x00A0, 0x3007, 0x2127, 0x00A1],[0x00F0, 0x1003, 0x0C21, 0x00AD, 0x303F, 0x052D, 0x00A1, 0x1683, 0x1303, 0x0186, 0x0008, 0x1283, 0x1303, 0x00AB, 0x087F, 0x00AC, 0x3001],[0x0100, 0x00FF, 0x3000, 0x2144, 0x3008, 0x00FF, 0x1283, 0x1303, 0x082C, 0x2144, 0x3007, 0x00FF, 0x1283, 0x1303, 0x082B, 0x2144, 0x0008],[0x0110, 0x1283, 0x1303, 0x00AA, 0x30F0, 0x1283, 0x1303, 0x0586, 0x3006, 0x00FF, 0x1283, 0x1303, 0x082A, 0x2144, 0x1283, 0x1303, 0x1786],[0x0120, 0x30FF, 0x0085, 0x0185, 0x30FF, 0x0085, 0x0185, 0x0008, 0x1283, 0x1303, 0x00A7, 0x01A8, 0x01A9, 0x0827, 0x0229, 0x1803, 0x2942],[0x0130, 0x1003, 0x0CA8, 0x1283, 0x1303, 0x1586, 0x30F0, 0x0586, 0x1B86, 0x293D, 0x1283, 0x1303, 0x13A8, 0x2940, 0x1283, 0x1303, 0x17A8],[0x0140, 0x0AA9, 0x292C, 0x0828, 0x0008, 0x1283, 0x1303, 0x00A4, 0x087F, 0x00A5, 0x01A6, 0x0825, 0x0226, 0x1803, 0x2960, 0x1824, 0x2954],[0x0150, 0x1283, 0x1303, 0x1386, 0x2957, 0x1283, 0x1303, 0x1786, 0x1586, 0x30F0, 0x0586, 0x1003, 0x1283, 0x1303, 0x0CA4, 0x0AA6, 0x294A],[0x0160, 0x0008, 0x1283, 0x1303, 0x00A3, 0x3000, 0x0423, 0x1903, 0x2973, 0x303D, 0x1283, 0x1303, 0x0081, 0x110B, 0x1D0B, 0x296D, 0x1283],[0x0170, 0x1303, 0x03A3, 0x2964, 0x0008, 0x3087, 0x1683, 0x1303, 0x0081, 0x0008, 0x1283, 0x1303, 0x00A2, 0x1283, 0x1303, 0x1E0C, 0x297C],[0x0180, 0x1283, 0x1303, 0x0822, 0x1283, 0x1303, 0x0099, 0x0008, 0x1283, 0x1303, 0x1E8C, 0x2987, 0x081A, 0x0008, 0x3081, 0x1683, 0x1303],[0x0190, 0x0099, 0x3024, 0x0098, 0x3090, 0x1283, 0x0098, 0x0008, 0x118A, 0x120A, 0x2805],]
