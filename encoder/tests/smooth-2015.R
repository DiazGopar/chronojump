#Call with:
#Rscript smooth-2015.R

source("../util.R")

#Modified from uncompress from capture.R. This try to eliminate the zeros of the signal and returns a position.
#A timestamp is needed
#converts data: "0*5 1 0 -1*3 2"
#into points like: (0, 0) , (5, 1) , (6, 1) , (7, 0) , (8, -1) , (9, -2), (10, 0)
getPositionFromChanges <- function(curveCompressed)
{
  chunks = unlist(strsplit(curveCompressed, " "))
  points = matrix(c(0,0), ncol=2)
  x = 0
  y = 0
  for(i in 1:length(chunks)) 
  {
    if(grepl("\\*",chunks[i])) {
      chunk = as.numeric(unlist(strsplit(chunks[i], "\\*"))) #from "0*1072" to: 0 1072 (as integers)
      if(chunk[1] == 0){
        x = x + chunk[2]
      } else {
        for( j in 1:chunk[2]){
          y = y + chunk[1]
          point = matrix(c(x, y), ncol=2)
          points <- rbind(points, point)
          x = x + 1
        }
      }
    } else  {
     {
       if(chunks[i] !=0 ){
         y = y + as.numeric(chunks[i])
         point = matrix(c(x, y), ncol=2)
         points <- rbind(points, point)
       }
      }
      x = x + 1
    }
  }
  return (points)
}

uncompress <- function(curveSent)
{
  chunks = unlist(strsplit(curveSent, " "))
  ints = NULL
  for(i in 1:length(chunks)) 
  {
    if(grepl("\\*",chunks[i])) {
      chunk = as.numeric(unlist(strsplit(chunks[i], "\\*"))) #from "0*1072" to: 0 1072 (as integers)
      chunk = rep(chunk[1],chunk[2])
    } else {
      chunk=chunks[i]
    }
    ints = c(ints,chunk)
  }
  return (as.numeric(ints))
}

getKinematicsFromCompressed <- function(curve, smoothing){
  positionDiscarded <- getPositionFromChanges(curve)
  
  #Adding extra points at de beggining and the end to make it flat
  pfirst <- matrix(c(-5:-1, rep(0, 5)), ncol=2)
  xlast <- positionDiscarded[nrow(positionDiscarded), 1] + 5
  ylast <- positionDiscarded[nrow(positionDiscarded), 2]
  plast <- matrix(c(seq(from=(xlast + 1), to=(xlast + 5)), rep(ylast,5)), ncol=2)
  positionDiscarded <- rbind(pfirst, positionDiscarded, plast)
  
  positionDiscardedSmoothed <- smooth.spline(positionDiscarded, spar=smoothing)
  position <- predict(positionDiscardedSmoothed, -5:positionDiscarded[nrow(positionDiscarded), 1], deriv=0)
  speed <- predict(positionDiscardedSmoothed, 0:positionDiscarded[nrow(positionDiscarded) -5 ,1], deriv=1)
  accel <- predict(positionDiscardedSmoothed, 0:positionDiscarded[nrow(positionDiscarded) -5 ,1], deriv=2)
  return(list(positionDiscarded=positionDiscarded, position=position$y[6:(length(position$y) - 5)], speed=speed$y, accel=accel$y*1000))
}

drawKinematics <- function(kinematics){
  par(mar=c(5,4,4,8))
  plot(kinematics$position, type="l")
  par(new=TRUE)
  plot(kinematics$speed, type="l", col="green", axes=FALSE, ylab=FALSE)
  axis(4, padj=-.5, col="green")
  par(new=TRUE)
  plot(kinematics$accel, type="l", col="blue", axes=FALSE, ylab=FALSE)
  axis(4, col="blue", padj=+1)
}

getSpeedByPosition <- function(position) {
	speed = list()
	speed$y <- diff(position)
	speed$x <- 1:length(speed$y)
	return(speed)
	
	#x <- 1:length(position)
	#speed <- predict(lm(position~x), deriv=1)

	#return(speed)
}

#Real displacement and the  resulting compressed curve
displacement = c(0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
curve <- "0*6 1 0*7 1 0*5 1 0*4 1 0*3 1 0*3 1 0*2 1 0*2 1 0 1 0*2 1 0 1 0 1 0 1*2 0 1 0 1*2 0 1*3 0 1*3 0 1*7 0 1*23 0 1*18 0 1*8 0 1*6 0 1*5 0 1*4 0 1*4 0 1*4 0 1*4 0 1*3 0 1*4 0 1*4 0 1*3 0 1*4 0 1*4 0 1*4 0 1*5 0 1*4 0 1*4 0 1*5 0 1*4 0 1*5 0 1*4 0 1*4 0 1*4 0 1*4 0 1*3 0 1*4 0 1*3 0 1*3 0 1*3 0 1*4 0 1*3 0 1*3 0 1*3 0 1*3 0 1*3 0 1*4 0 1*3 0 1*3 0 1*3 0 1*3 0 1*3 0 1*3 0 1*3 0 1*3 0 1*3 0 1*3 0 1*2 0 1*3 0 1*3 0 1*3 0 1*2 0 1*3 0 1*2 0 1*3 0 1*2 0 1*3 0 1*2 0 1*3 0 1*2 0 1*2 0 1*3 0 1*2 0 1*2 0 1*2 0 1*2 0 1*3 0 1*2 0 1*2 0 1*2 0 1*2 0 1*2 0 1*2 0 1*2 0 1*2 0 1 0 1*2 0 1*2 0 1*2 0 1*2 0 1 0 1*2 0 1*2 0 1*2 0 1 0 1*2 0 1 0 1*2 0 1*2 0 1 0 1*2 0 1 0 1*2 0 1 0 1*2 0 1 0 1*2 0 1 0 1*2 0 1 0 1*2 0 1 0 1 0 1*2 0 1 0 1 0 1*2 0 1 0 1 0 1*2 0 1 0 1 0 1 0 1*2 0 1 0 1 0 1 0 1 0 1*2 0 1 0 1 0 1 0 1 0 1 0 1 0 1*2 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0 1 0*2 1 0 1 0*2 1 0*2 1 0*3 1 0*5 1 0*10"

position <- getPositionFromCompressed(curve, 0.7)
plot(position, type="l")
speed <- getSpeedByPosition(position)
plot(speed$y, type="l")