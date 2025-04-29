import axios from 'axios';

const api = axios.create({
  // baseURL: 'http://localhost:5000',
});

export interface PersonAstronaut {
  personId: number;
  name: string;
  currentRank: string;
  currentDutyTitle: string;
  careerStartDate: string | null;
  careerEndDate: string | null;
}

export interface AstronautDuty {
  id: number;
  personId: number;
  rank: string;
  dutyTitle: string;
  dutyStartDate: string;
  dutyEndDate: string | null;
}

export interface GetAstronautDutiesResponse {
  success: boolean;
  message: string;
  responseCode: number;
  person: PersonAstronaut;
  astronautDuties: AstronautDuty[];
}

export const fetchAstronautDutiesByName = async (name: string): Promise<GetAstronautDutiesResponse> => {
  const response = await api.get<GetAstronautDutiesResponse>(`/AstronautDuty/${encodeURIComponent(name)}`);
  return response.data;
};

export default api;