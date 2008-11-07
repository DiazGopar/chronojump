/*
 * This file is part of ChronoJump
 *
 * ChronoJump is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or   
 *    (at your option) any later version.
 *    
 * ChronoJump is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
 *    GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 *
 * Initally coded by (v.1.0):
 * Sharad Shankar & Onkar Nath Mishra
 * http://www.logicbrick.com/
 * 
 * Updated by:
 * Xavier de Blas 
 * xaviblas@gmail.com
 *
 *
 */




#include "opencv/cv.h"
#include "opencv/highgui.h"
#include <opencv/cxcore.h>
#include "stdio.h"
#include "math.h"
#include<iostream>
#include<fstream>
#include<vector>
#include <string>
using namespace std;


CvScalar WHITE = 	CV_RGB(255,255,255);
CvScalar BLACK = 	CV_RGB(0  ,0  ,  0);
CvScalar RED =		CV_RGB(255,  0,  0);
CvScalar GREEN = 	CV_RGB(0  ,255,  0);
CvScalar BLUE = 	CV_RGB(0  ,0  ,255);
CvScalar GREY = 	CV_RGB(128,128,128);
CvScalar YELLOW = 	CV_RGB(255,255,  0);
CvScalar MAGENTA = 	CV_RGB(255,  0,255);
CvScalar CYAN = 	CV_RGB(  0,255,255); //check

enum { SMALL = 1, MID = 2, BIG = 3 };


CvPoint findMiddle(CvPoint pt1, CvPoint pt2)
{
	return cvPoint((pt1.x+pt2.x)/2, (pt1.y+pt2.y)/2);
}

CvPoint findCenter(CvPoint pt1, CvPoint pt2)
{
	CvPoint center;
	center.x=(pt1.x + pt2.x)/2 +1;
	center.y=(pt1.y + pt2.y)/2 +1;
	return center;
}

bool connectedWithPoint(CvPoint pt, CvPoint sp)
{
	if     (sp.x == pt.x -1 && sp.y == pt.y) //left
		return true;
	else if(sp.x == pt.x -1 && sp.y == pt.y-1) //upLeft
		return true;
	else if(sp.x == pt.x    && sp.y == pt.y-1) //up
		return true;
	else if(sp.x == pt.x +1 && sp.y == pt.y-1) //upRight
		return true;
	else if(sp.x == pt.x +1 && sp.y == pt.y  ) //right
		return true;
	else if(sp.x == pt.x +1 && sp.y == pt.y+1) //downRight
		return true;
	else if(sp.x == pt.x    && sp.y == pt.y+1) //down
		return true;
	else if(sp.x == pt.x -1 && sp.y == pt.y+1) //downLeft
		return true;
	//		else if(sp.y == pt.y -1) //special attention
	//			return true;
	else
		return false;
}

CvRect seqRect(CvSeq *seq) {

	int minx = 1000;
	int miny = 1000;
	int maxx = 0;
	int maxy = 0;
	for( int i = 0; i < seq->total; i++ )
	{
		CvPoint pt = *CV_GET_SEQ_ELEM( CvPoint, seq, i ); //seqPoint
		if(pt.x < minx)
			minx = pt.x;
		if(pt.x > maxx)
			maxx = pt.x;
		if(pt.y < miny)
			miny = pt.y;
		if(pt.y > maxy)
			maxy = pt.y;
	}
	
	CvRect rect;

	rect.x = minx;
	rect.y = miny;
	rect.width = maxx-minx;
	rect.height = maxy-miny;

	return rect;
}


CvPoint findCorner(CvSeq* seq, bool first)
{
	int minx= 1000;
	int miny= 1000;
	int maxx= 0;
	int maxy= 0;
	CvPoint pt;
	if(first) {
		pt.x=minx; pt.y=miny;
	} else {
		pt.x=maxx; pt.y=maxy;
	}

	for( int i = 0; i < seq->total; i++ ) {
		CvPoint sp = *CV_GET_SEQ_ELEM( CvPoint, seq, i ); //seqPoint
		if(first) {
			if(sp.x < pt.x) 
				pt.x = sp.x;
			if(sp.y < pt.y) 
				pt.y = sp.y;
		} else {
			if(sp.x > pt.x)
				pt.x = sp.x;
			if(sp.y > pt.y)
				pt.y = sp.y;
		}
	}
	return pt;
}

/* at first photogramm where knee or foot is detected (it will not be too horizontal) find it's width and use all the time to fix kneex
 * at knee is called only done one time (because in max flexion, the back is line with the knee and there will be problems knowing knee width
 * at foot is called all the time
 */
int findWidth(IplImage* img, CvPoint point, bool goRight)
{
	CvMat *srcmat,src_stub;
	srcmat = cvGetMat(img,&src_stub);
	uchar *srcdata = srcmat->data.ptr;
	int width = img->width;

	int y=point.y;

	uchar *srcdataptr = srcdata + y*img->width;
	int countX = 0;

	if(goRight)
		for(int x=point.x+1; srcdataptr[x]; x++)
			countX ++;
	else
		for(int x=point.x-1; srcdataptr[x]; x--)
			countX ++;

	return countX;
}

double abs(double val)
{
	if(val<0)
		val *= -1;
	return val;
}

double getDistance(CvPoint p1, CvPoint p2)
{
	return sqrt( pow(p1.x-p2.x, 2) + pow(p1.y-p2.y, 2) );
}

double getDistance3D(CvPoint p1, CvPoint p2, int p1z, int p2z)
{
	return sqrt( pow(p1.x-p2.x, 2) + pow(p1.y-p2.y, 2) + pow(p1z-p2z, 2) );
}

int checkItsOk(int val, int min, int max)
{
	if(val < min)
		return min;
	else if(val > max)
		return max;
	return val;
}


bool upperSimilarThanLower(CvPoint hipPoint, CvPoint kneePoint, CvPoint footPoint)
{
	double upper = getDistance(kneePoint, hipPoint); 
	double lower = getDistance(kneePoint, footPoint);
	double big = 0; 
	double little = 0;
	
	if(upper > lower) {
		big = upper;
		little = lower;
	} else {
		big = lower;
		little = upper;
	}

//	if(debug)
//		printf("upper(%.1f), lower(%.1f), big/little (%.2f)\n",upper, lower, big/(double)little);

	//if one is not n times or more bigger than the other
	//consider that if camera hides shoes and a bit more up, 2 times is risky in a maximal flexion
	//consider also that this data is previous to fixing
	double n = 2.5;
	if(big / (double)little < n)
		return true;
	else 
		return false;
}

bool pointIsNull(CvPoint pt) {
	CvPoint notFoundPoint;
	notFoundPoint.x = 0; notFoundPoint.y = 0;
	if(pt.x == notFoundPoint.x && pt.y == notFoundPoint.y) 
		return true;
	else 
		return false;
}

bool pointInside(CvPoint pt, CvPoint upLeft, CvPoint downRight ) {
	if(
			pt.x >= upLeft.x && pt.x <= downRight.x && 
			pt.y >= upLeft.y && pt.y <= downRight.y)
		return true;
	return false;
}

double findAngle2D(CvPoint p1, CvPoint p2, CvPoint pa) //pa is the point at the angle
{
	CvPoint d1, d2;
	d1.x = p1.x - pa.x;
	d1.y = p1.y - pa.y;
	d2.x = p2.x - pa.x;
	d2.y = p2.y - pa.y;
	double dist1 = getDistance(p1, pa);
	double dist2 = getDistance(p2, pa);
	return (180.0/M_PI)*acos(((d1.x*d2.x + d1.y*d2.y))/(double)(dist1*dist2));
}

double findAngle3D(CvPoint p1, CvPoint p2, CvPoint pa, int p1z, int p2z, int paz) //pa is the point at the angle
{
	CvPoint d1, d2;
	d1.x = p1.x - pa.x;
	d1.y = p1.y - pa.y;
	int d1z = p1z - paz;
	d2.x = p2.x - pa.x;
	d2.y = p2.y - pa.y;
	int d2z = p2z - paz;
	double dist1 = getDistance3D(p1, pa, p1z, paz);
	double dist2 = getDistance3D(p2, pa, p2z, paz);
	return (180.0/M_PI)*acos(((d1.x*d2.x + d1.y*d2.y + d1z*d2z))/(double)(dist1*dist2));
}

double relError(double val1, double val2)
{
	if(val2-val1 == 0 || val2 == 0)
		return -1;
	else
		return (double) (val2-val1)/val2 *100;
}

int getMaxValue(CvSeq* seqGroups)
{
	int max = 0;
	for( int i = 0; i < seqGroups->total; i++ ) {
		int group = *CV_GET_SEQ_ELEM( int, seqGroups, i ); 
		if(group > max)
			max = group;
	}
	return max;
}
					
void fusionateGroups(CvSeq* seqGroups, int g1, int g2) {
	for( int i = 0; i < seqGroups->total; i++ ) {
		int found = *CV_GET_SEQ_ELEM( int, seqGroups, i );
		if(found == g2) {
			cvSeqRemove(seqGroups, i);
			cvSeqInsert(seqGroups, i, &g1);
		}
	}
}

int getGroup(int pointPos, CvPoint pt, CvSeq* seqPoints, CvSeq* seqGroups)
{
	//search in previous points
	int group = -1;
	for( int i = 0; i < pointPos; i++ ) {
		CvPoint sp = *CV_GET_SEQ_ELEM( CvPoint, seqPoints, i ); 
		if(connectedWithPoint(pt, sp)) { //if is connected with 
			int tempGroup = *CV_GET_SEQ_ELEM( int, seqGroups, i );
			if(group != -1 && //a group connected with that point was found
					tempGroup != group) //that group was not the same group we just found now
			{ 
				//it was connected with another group, let's fusionate
				fusionateGroups(seqGroups, tempGroup, group);
			}
			group = *CV_GET_SEQ_ELEM( int, seqGroups, i );
		}
	}

	if(pointPos == 0) //return 0 is the first point
		return 0;
	else if(group == -1) //if not found, return a new group
		return getMaxValue(seqGroups) +1;
	else
		return group; //return group found
}

CvPoint pointToZero() {
	CvPoint point;
	point.x=0; point.y=0; 
	return point;
}

enum { NO = 0, YES = 1, FORWARD = 2, SUPERFORWARD = 3, QUIT = 4 };

int optionAccept() {
	int key;
	do {
		key = (char) cvWaitKey(0);
	} while(key != 'n' && key != 'y' && key != 'f' && key != 'F' && key != 'q');

	if(key == 'n') 
		return NO;
	else if(key == 'y') 
		return YES;
	else if(key == 'f') 
		return FORWARD;
	else if(key == 'F') 
		return SUPERFORWARD;
	else if(key == 'q') 
		return QUIT;
}

void imagePrint(IplImage *img, const char * imgName, CvPoint point, const char *label, CvFont font, CvScalar color) {
	cvPutText(img, label, point, &font, color);
}

void imageGuiAsk(IplImage *gui, const char *labelQuestion, const char * labelOptions, CvFont font) {
	imagePrint(gui, "gui", cvPoint(25, gui->height-40), labelQuestion, font, WHITE);
	imagePrint(gui, "gui", cvPoint(25, gui->height-20), labelOptions, font, WHITE);
	
	cvShowImage("gui", gui);
}

void eraseGuiAsk(IplImage * gui) {
	cvRectangle(gui, cvPoint(0, gui->height-50), cvPoint(gui->width, gui->height), CV_RGB(0,0,0),CV_FILLED,8,0);
	cvShowImage("gui", gui);
}

void eraseGuiWindow(IplImage * gui) {
	cvRectangle(gui, cvPoint(0, 0), cvPoint(gui->width, gui->height), CV_RGB(0,0,0),CV_FILLED,8,0);
	cvShowImage("gui", gui);
}



void crossPoint(IplImage * img, CvPoint point, CvScalar color, int sizeEnum) {
	int size;
	if(sizeEnum == SMALL)
		size = 6;
	else if(sizeEnum == MID)
		size = 10;
	else // if(sizeEnum == BIG)
		size = 14;

	cvLine(img,
			cvPoint(point.x - size/2, point.y),
			cvPoint(point.x + size/2, point.y),
			color,1,1);
	cvLine(img,
			cvPoint(point.x, point.y - size/2),
			cvPoint(point.x, point.y + size/2),
			color,1,1);
}
