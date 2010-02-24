@echo off
fxc Effects\terrain.vs /T vs_2_0 /E main /O3 /Fo Effects\terrain.cvs /Zpr
fxc Effects\terrain.ps /T ps_2_0 /E main /O3 /Fo Effects\terrain.cps /Zpr

fxc Effects\water.vs /T vs_2_0 /E main /O3 /Fo Effects\water.cvs /Zpr
fxc Effects\water.ps /T ps_2_0 /E main /O3 /Fo Effects\water.cps /Zpr

fxc Effects\skybox.vs /T vs_2_0 /E main /O3 /Fo Effects\skybox.cvs /Zpr
fxc Effects\skybox.ps /T ps_2_0 /E main /O3 /Fo Effects\skybox.cps /Zpr

fxc Effects\standard.vs /T vs_2_0 /E main /O3 /Fo Effects\standard.cvs /Zpr
fxc Effects\standard.ps /T ps_2_0 /E main /O3 /Fo Effects\standard.cps /Zpr
