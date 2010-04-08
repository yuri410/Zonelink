@echo off
fxc Effect\terrain.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\terrain.cvs /Zpr
fxc Effect\terrain.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\terrain.cps /Zpr

fxc Effect\water.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\water.cvs /Zpr
fxc Effect\water.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\water.cps /Zpr

fxc Effect\skybox.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\skybox.cvs /Zpr
fxc Effect\skybox.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\skybox.cps /Zpr

fxc Effect\standard.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\standard.cvs /Zpr
fxc Effect\standard.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\standard.cps /Zpr

fxc Effect\earthbase.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\earthbase.cvs /Zpr
fxc Effect\earthbase.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\earthbase.cps /Zpr

fxc Effect\atmosphere.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\atmosphere.cvs /Zpr
fxc Effect\atmosphere.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\atmosphere.cps /Zpr

fxc Effect\citylink.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\citylink.cvs /Zpr
fxc Effect\citylink.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\citylink.cps /Zpr

fxc Effect\cityring.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\cityring.cvs /Zpr
fxc Effect\cityring.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\cityring.cps /Zpr

fxc Effect\tree.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\tree.cvs /Zpr
fxc Effect\tree.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\tree.cps /Zpr

fxc Effect\particle.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\particle.cvs /Zpr
fxc Effect\particle.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\particle.cps /Zpr

fxc Effect\road.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\road.cvs /Zpr
fxc Effect\road.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\road.cps /Zpr

fxc Effect\cloud.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\cloud.cvs /Zpr
fxc Effect\cloud.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\cloud.cps /Zpr

fxc Post\composite.ps.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\composite.cps /Zpr
fxc Post\bloom.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\bloom.cps /Zpr
fxc Post\blurX.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\blurX.cps /Zpr
fxc Post\blurY.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\blurY.cps /Zpr
fxc Post\edge.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\edge.cps /Zpr
fxc Post\postQuad.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\postQuad.cvs /Zpr
