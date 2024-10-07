import axios from 'axios';
import { toast } from 'react-hot-toast';

export const useErrorHandler = () => {
    const handleError = (error: any) => {
        if (axios.isAxiosError(error) && error.response) {
            const problemDetails = error.response.data;
            
            if (error.response.status === 400 && problemDetails.errors) {
                const validationErrors = Object.values(problemDetails.errors)
                    .map((messages) => (messages as string[])[0])
                    .join('; ');
                toast.error(validationErrors);
            } else if (problemDetails.detail) {
                toast.error(`${problemDetails.detail}`);
            } else {
                toast.error('An unknown error occurred.');
            }
        } else {
            toast.error('An unknown error occurred.');
        }
    };

    return { handleError };
};