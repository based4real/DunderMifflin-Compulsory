import axios from 'axios';
import { toast } from 'react-hot-toast';

export const useErrorHandler = () => {
    const handleError = (error: any) => {
        if (!axios.isAxiosError(error) || !error.response) {
            return toast.error('An unknown error occurred.');
        }

        const { status, data } = error.response;
        const { detail, errors } = data;

        if (status === 400 && errors) {
            const validationErrors = Object.values(errors).flat().join('; ');
            return toast.error(validationErrors);
        }

        return toast.error(detail || 'An unknown error occurred.');
    };

    return { handleError };
};