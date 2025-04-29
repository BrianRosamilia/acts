import { Button, Input, Flex } from '@chakra-ui/react';
import { useState, type FormEvent, type ChangeEvent } from 'react';

interface SearchAstronautProps {
  onSearch: (name: string) => void;
  isLoading: boolean;
}

export function SearchAstronaut({ onSearch }: SearchAstronautProps) {
  const [name, setName] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setIsLoading(true);
    try {
      await onSearch(name);
    } catch (error) {
      // Simple error handling without toast for now
      console.error('Error searching astronaut:', error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <Flex>
        <Input 
          placeholder="Enter astronaut name"
          value={name}
          onChange={(e: ChangeEvent<HTMLInputElement>) => setName(e.target.value)}
          borderRightRadius={0}
          size="lg"
        />
        <Button
          colorPalette="blue"
          size="lg"
          loading={isLoading}
          loadingText="Searching"
          type="submit"
          borderLeftRadius={0}
          ml={0}
        >
          Search
        </Button>
      </Flex>
    </form>
  );
}