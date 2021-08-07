[] - Some reason "Start Game" only works 80% of the time.

[] -  We now are properly delievering game tasks (locations and what not).  Filtering should probably happen before sending the data to limit data transfer.  That use locations to filter tasks and then send the list of tasks or even the correct number of randomized tasks.

[] - We also need to fix the thing where the station drop down is getting a non-unique list of locations.

[] - GameController is still trying to communicate with the Java server.  Must comm with the mirror server.