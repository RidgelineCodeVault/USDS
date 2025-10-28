https://www.ecfr.gov/

1. download the current eCFR data*
2. store the data server-side*
3. create APIs that can retrieve the server-side stored data*
4. provide a UI to analyze it for items such as*
	a. word count per agency*
	b. historical changes over time*
	c. a checksum for each agency *

•	Only implement analysis that would provide meaningful information to the user. 
•	Please add at least one of your own custom metrics 
	that you believe may help inform decision-making more effectively. 

NOTES:
1. I tried to keep the amount of time to what was required 4-6 hours
2. It would be helpful to have an XSLT Transform document for display, JSON or the like, which would take more time. 
3. I would have liked more time to study the data itself and develop a frame of reference
4. I left stubs in here to show what else could be added
5. This could easily be modified to leverage more years of data 
6. Could have used a database and tables instead of the file system, which would have required some data access code
7. A search option could easily be added
8. This is more of a coding and less of a data analysis exercise
9. I added the checksum, but it wasn't fully implemented for time 
10. Display of historical changes to the data wasn't as robust as I'd have liked 

GitHub: https://github.com/RidgelineCodeVault/USDS
