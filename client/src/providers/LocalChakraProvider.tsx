import {
    ChakraProvider,
    createSystem,
    defaultConfig,
    defineConfig,
  } from "@chakra-ui/react"
import { ReactNode } from "react"
  
  const config = defineConfig({
    theme: {
      tokens: {
        colors: {},
      },
    },
  })
  
  const system = createSystem(defaultConfig, config)
  
export function LocalChakraProvider({ children }: { children: ReactNode }) {
  return (
    <ChakraProvider value={system}>
      {children}
    </ChakraProvider>
  );
}