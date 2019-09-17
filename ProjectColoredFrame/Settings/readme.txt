Testing custom saving/loading of settings may not work.
At the time of writing this User Settings Store is not remembered by the Experimental instance.
It affects array-like settings: Custom Palette and Custom Mapping.
Workaround 1: Install the vsix from bin folder into the normal instance and test in it.
Workaround 2: The test is only neccessary to check if settings persist on close/launch of VS. Everything else can be just tested without closing VS.
