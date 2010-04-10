@echo off
fxc Effect\terrain.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\terrain.cvs /Zpr /WX /nologo
fxc Effect\terrain.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\terrain.cps /Zpr /WX /nologo

fxc Effect\water.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\water.cvs /Zpr /WX /nologo
fxc Effect\water.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\water.cps /Zpr /WX /nologo

fxc Effect\skybox.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\skybox.cvs /Zpr /WX /nologo
fxc Effect\skybox.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\skybox.cps /Zpr /WX /nologo

fxc Effect\standard.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\standard.cvs /Zpr /WX /nologo
fxc Effect\standard.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\standard.cps /Zpr /WX /nologo

fxc Effect\earthbase.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\earthbase.cvs /Zpr /WX /nologo
fxc Effect\earthbase.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\earthbase.cps /Zpr /WX /nologo

fxc Effect\atmosphere.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\atmosphere.cvs /Zpr /WX /nologo
fxc Effect\atmosphere.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\atmosphere.cps /Zpr /WX /nologo

fxc Effect\citylink.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\citylink.cvs /Zpr /WX /nologo
fxc Effect\citylink.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\citylink.cps /Zpr /WX /nologo

fxc Effect\cityring.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\cityring.cvs /Zpr /WX /nologo
fxc Effect\cityring.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\cityring.cps /Zpr /WX /nologo

fxc Effect\tree.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\tree.cvs /Zpr /WX /nologo
fxc Effect\tree.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\tree.cps /Zpr /WX /nologo

fxc Effect\particle.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\particle.cvs /Zpr /WX /nologo
fxc Effect\particle.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\particle.cps /Zpr /WX /nologo

fxc Effect\cloud.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\cloud.cvs /Zpr /WX /nologo
fxc Effect\cloud.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\cloud.cps /Zpr /WX /nologo

fxc Post\composite.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\composite.cps /Zpr /WX /nologo
fxc Post\bloom.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\bloom.cps /Zpr /WX /nologo
fxc Post\blur.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\blur.cps /Zpr /WX /nologo
fxc Post\edge.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\edge.cps /Zpr /WX /nologo
fxc Post\postQuad.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\postQuad.cvs /Zpr /WX /nologo



fxc ShadowMap\shadowMap.ps /T ps_2_0 /E main /O3 /Fo ..\Code2015\Effect\shadowMap.cps /Zpr /WX /nologo
fxc ShadowMap\shadowMap.vs /T vs_2_0 /E main /O3 /Fo ..\Code2015\Effect\shadowMap.cvs /Zpr /WX /nologo
