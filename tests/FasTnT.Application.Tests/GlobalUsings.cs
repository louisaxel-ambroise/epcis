global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;

[assembly: Parallelize(Scope = ExecutionScope.ClassLevel)]