# Enabling Collision

Attach a __Collider 2D__ component to your Sprite Shape to enable the Collider properties in the __Sprite Shape Controller.__ Only the __Edge__ and __Polygon Collider 2D__ components can be used with __Sprite Shapes__.

![Attaching a Collider component](images/v1.1-Collider.png)

The Collider mesh automatically updates itself to the shape of the Sprite Shape when attached. See the Collider section of the [Sprite Shape Controller](SSController.md) page for more details about the Sprite Shape Collider options.

By default, the Collider mesh is automatically reshaped to match the Sprite Shape every time it is edited. To make manual edits to the Collider mesh directly, first disable both __Update Collider__ and __Optimize Collider__ in the __Sprite Shape Controller's__ Collider settings to prevent the Controller from updating the Collider mesh automatically and overriding your manual edits.

![Disabling the Collider options](images/v1.1-ColliderOptionsDisable.png)

You can now edit the Collider mesh independent of the shape of the __Sprite Shape.__

