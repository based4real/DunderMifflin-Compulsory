import path from "node:path";
import { generateApi } from "swagger-typescript-api";

generateApi({
    name: "Api.ts",
    output: path.resolve(process.cwd(), "./src/"),
    url: "http://localhost:5009/swagger/v1/swagger.json",
    httpClientType: "axios",
    moduleNameIndex: 1, // Lav så man har api.paper, api.order, api.customer, etc.
    hooks: {
        onFormatRouteName: (routeInfo, templateRouteName) => {
            const { moduleName } = routeInfo;
            let name = templateRouteName;

            // Fjern modul navn prefix 'paperCreatePaper' til 'CreatePaper'
            if (moduleName && name) {
                const moduleNameLower = moduleName.toLowerCase();
                if (name.toLowerCase().startsWith(moduleNameLower)) {
                    name = name.substring(moduleName.length);
                }
            }

            // Lav om til camelCase 'CreatePaper' til 'createPaper'
            if (name) {
                name = name.charAt(0).toLowerCase() + name.slice(1);
            } else {
                console.warn("Method name is undefined for route:", routeInfo.route);
                name = "unnamedMethod";
            }

            return name;
        },
    },
}).catch((e) => console.error(e));