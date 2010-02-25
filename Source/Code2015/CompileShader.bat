@echo off
fxc Effect.lpk\terrain.vs /T vs_2_0 /E main /O3 /Fo Effect.lpk\terrain.cvs /Zpr
fxc Effect.lpk\terrain.ps /T ps_2_0 /E main /O3 /Fo Effect.lpk\terrain.cps /Zpr

fxc Effect.lpk\water.vs /T vs_2_0 /E main /O3 /Fo Effect.lpk\water.cvs /Zpr
fxc Effect.lpk\water.ps /T ps_2_0 /E main /O3 /Fo Effect.lpk\water.cps /Zpr

fxc Effect.lpk\skybox.vs /T vs_2_0 /E main /O3 /Fo Effect.lpk\skybox.cvs /Zpr
fxc Effect.lpk\skybox.ps /T ps_2_0 /E main /O3 /Fo Effect.lpk\skybox.cps /Zpr

fxc Effect.lpk\standard.vs /T vs_2_0 /E main /O3 /Fo Effect.lpk\standard.cvs /Zpr
fxc Effect.lpk\standard.ps /T ps_2_0 /E main /O3 /Fo Effect.lpk\standard.cps /Zpr
