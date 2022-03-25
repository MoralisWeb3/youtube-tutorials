The demo scenes are using linear color space and post-processing stack v2 for better visual quality. However, if you are developing a mobile game, please disable post-processing stack, or use faster alternatives.

If the campfire particles are shown up on mobile platforms, please use mobile particle shader for FireParticleMat / FireParticleAddMat instead.

LWRP/URP are not support, but it's easy to convert these materials into these rendering pipelines, since these materials are using standard shader. However, you will want to use alternative water shader for LWRP/URP.