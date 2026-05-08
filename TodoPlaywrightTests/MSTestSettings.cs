// Playwright-Tests müssen auf ClassLevel oder höher parallelisiert werden,
// da jede Klasse ihren eigenen Browser-Context hat.
// Bei MethodLevel würden mehrere Tests denselben Page-Context teilen → Race Conditions.
[assembly: Parallelize(Scope = ExecutionScope.ClassLevel)]
