import { Box, Container, Heading, Text } from '@chakra-ui/react';
import { useQuery, QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useState } from 'react';
import { AstronautTimeline } from './components/AstronautTimeline';
import { SearchAstronaut } from './components/SearchAstronaut';
import { LocalChakraProvider } from './providers/LocalChakraProvider';
import { fetchAstronautDutiesByName, type GetAstronautDutiesResponse } from './services/api';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      refetchOnWindowFocus: false,
      staleTime: 5 * 60 * 1000,
    },
  },
});

function AstronautDutyApp() {
  const [astronautName, setAstronautName] = useState<string | null>(null);
  
  const { 
    isLoading, 
    data, 
    error, 
    isError 
  } = useQuery<GetAstronautDutiesResponse, Error>({
    queryKey: ['astronautDuties', astronautName],
    queryFn: () => fetchAstronautDutiesByName(astronautName!),
    enabled: !!astronautName,
  });

  const handleSearch = (name: string) => {
    setAstronautName(name);
  };

  return (
    <Box minH="100vh" py={10}>
      <Container maxW="900px" mx="auto">
        <Box textAlign="left" mb={10}>
          <Heading as="h1" size="2xl" mb={2}>Astronaut Career Tracking System</Heading>
        </Box>

        <SearchAstronaut onSearch={handleSearch} isLoading={isLoading} />
        
        {isError && (error as any)?.response?.status === 500 && (
          <Box mb={6} p={4} borderRadius="md" bg="red.100" color="red.900">
            <Text>{error?.message || 'Failed to load astronaut data'}</Text>
          </Box>
        )}
        
        {astronautName ? (
          <AstronautTimeline 
            isLoading={isLoading} 
            person={data?.person} 
            duties={data?.astronautDuties} 
          />
        ) : (
          <Box p={6} textAlign="center" mt={"30px"} borderRadius="lg" bg="white" boxShadow="md">
            <Text color="gray.600">Enter an astronaut name to view their duty timeline</Text>
          </Box>
        )}
      </Container>
    </Box>
  );
}

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <LocalChakraProvider>
        <AstronautDutyApp />
      </LocalChakraProvider>
    </QueryClientProvider>
  );
}
