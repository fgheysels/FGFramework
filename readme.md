A long time ago, in the early nillies (it must have been 2004 I think), I've read a book of Rockford Lhotka titled ["Expert C# Business Objects"](https://www.apress.com/br/book/9781430207375) in where he explains his CSLA.NET Business Object framework.
This project of me was an attempt to improve the CSLA.NET framework as it existed back then.

As I felt it, the data-access logic was too tightly coupled with the Business Objects in the CSLA.NET framework, so I decided to decouple that.  I also added support for lazy loading of collections by applying the proxy pattern as explained in Martin Fowlers' [Patters of Enterprise Application Architecture](https://martinfowler.com/eaaCatalog/index.html).

As the years have moved along and with the dawn of O/R mapping tools like NHibernate and Entity Framework, this project is now completely deprecated and should not be used in production code.
It was however fun to write and nice study material and for the sake of history, I've placed it on GitHub.