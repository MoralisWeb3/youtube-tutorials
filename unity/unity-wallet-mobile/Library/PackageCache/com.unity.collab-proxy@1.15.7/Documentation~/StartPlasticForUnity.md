# Getting started with Plastic SCM for Unity

Plastic SCM for Unity will allow you to use Plastic SCM directly in Unity and is available via the Version Control package in the Unity Package Manager.

You will need to uninstall the Plastic SCM Asset Store plugin from your Unity project in order to use Plastic SCM for Unity via the Version Control package. Here are the steps to do so:

1. Navigate to your Assets/Plugins folder in your Unity Project.
Delete the PlasticSCM folder.
2. Re-open your Unity Project to recompile your packages.

**Note**: The .plastic workspace folder in the root of the Unity Project does not need to be removed as it can be read by the Version Control package.

**Important**: Enabling the Plastic SCM for Unity package will disable the standard VCS integration option in the Project Settings (this option will be removed from future Editor releases), which was an older integration previously offered in the Editor. Features available in the standard integration have been ported to the Plastic SCM for Unity package. This is the version that will be actively developed and maintained going forward.

Learn more about [Plastic SCM Cloud Edition](https://unity.com/products/plastic-scm).

* To start with a new Plastic SCM repository for your project, see [Getting started with a New Plastic SCM repository](NewPlasticRepo.md)
* To start from an existing Plastic SCM repository, see [Getting started with an existing Plastic SCM repository](ExistingPlasticRepo.md)