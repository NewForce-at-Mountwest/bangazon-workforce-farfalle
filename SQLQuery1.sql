SELECT e.Id AS 'Employee Id', e.FirstName, e.LastName, e.IsSuperVisor, e.DepartmentId,
                        d.Id AS 'Department Id', d.Name AS 'Department', d.Budget ,c.Id AS 'Computer Id', tp.Name AS 
						ProgramName,
						c.Make, c.Manufacturer, c.PurchaseDate, c.DecomissionDate, tp.Id AS 'Training Id'
                        FROM Employee e LEFT JOIN Department d ON e.DepartmentId = d.Id
						LEFT JOIN ComputerEmployee ce ON e.Id = ce.EmployeeId
                        LEFT JOIN Computer c ON ce.ComputerId=c.Id LEFT JOIN EmployeeTraining et ON e.Id = et.EmployeeId 
						LEFT JOIN 
						TrainingProgram tp 
						ON et.TrainingProgramId = tp.Id WHERE e.Id = 4