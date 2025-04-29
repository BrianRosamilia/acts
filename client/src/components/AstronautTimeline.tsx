import {
  Box,
  Flex,
  Text,
  Circle,
  Heading,
  Badge,
  Spinner,
} from '@chakra-ui/react';
import { Timeline } from '@chakra-ui/react';
import { AstronautDuty, PersonAstronaut } from '../services/api';

interface AstronautTimelineProps {
  isLoading: boolean;
  person?: PersonAstronaut;
  duties?: AstronautDuty[];
}

export function AstronautTimeline({ isLoading, person, duties }: AstronautTimelineProps) {
  if (isLoading) {
    return (
      <Flex justify="center" alignItems="center" h="400px">
        <Spinner size="xl" color="brand.500" />
      </Flex>
    );
  }
  
  if (!person || !duties || duties.length === 0) {
    return (
      <Box p={6} borderRadius="lg" boxShadow="md">
        <Text textAlign="center">No astronaut duty data available.</Text>
      </Box>
    );
  }
  
  return (
    <Box p={6} borderRadius="lg" boxShadow="md">
      <Heading size="lg" mb={6}>{person.name} - Astronaut</Heading>
      <Flex mb={6}>
        <Box flex={1}>
          <Text fontWeight="bold">Current Rank: </Text>
          <Badge colorScheme="blue" fontSize="md">{person.currentRank}</Badge>
        </Box>
        <Box flex={1}>
          <Text fontWeight="bold">Current Position: </Text>
          <Badge colorScheme="green" fontSize="md">{person.currentDutyTitle}</Badge>
        </Box>
        <Box flex={1}>
          <Text fontWeight="bold">Career Start: </Text>
          <Text>{person.careerStartDate ? new Date(person.careerStartDate).toLocaleDateString() : 'N/A'}</Text>
        </Box>
      </Flex>
      
      <Box h="1px" bg="gray.200" mb={6} />
      
      <Timeline.Root colorPalette="blue" size="md">
        {duties.map((duty, index) => (
          <Timeline.Item key={duty.id}>
            <Timeline.Connector>
              <Timeline.Separator />
              <Timeline.Indicator>
                <Circle size="40px" fontWeight="bold" bg="blue.500" color="white">
                  {index + 1}
                </Circle>
              </Timeline.Indicator>
            </Timeline.Connector>
            <Timeline.Content>
              <Timeline.Title>
                <Flex justifyContent="space-between" flexWrap="wrap">
                  <Box>
                    <Badge colorScheme="blue" mb={2}>{duty.rank}</Badge>
                    <Heading size="md">{duty.dutyTitle}</Heading>
                  </Box>
                </Flex>
              </Timeline.Title>
              <Timeline.Description>
                <Text fontWeight="bold">
                  {new Date(duty.dutyStartDate).toLocaleDateString()} - {
                    duty.dutyEndDate ? new Date(duty.dutyEndDate).toLocaleDateString() : 'Present'
                  }
                </Text>
              </Timeline.Description>
            </Timeline.Content>
          </Timeline.Item>
        ))}
      </Timeline.Root>
    </Box>
  );
}