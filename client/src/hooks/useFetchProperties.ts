import { useEffect } from 'react';
import { useAtom } from 'jotai';
import { api } from '../http';
import { PaperPropertiesSummaryAtom } from '../atoms/PaperPropertiesSummaryAtom';
import { IsBackendReachableAtom } from '../atoms/IsBackendReachableAtom';
import { useErrorHandler } from './useErrorHandler';

export function useFetchProperties(refresh?: boolean) {
    const { handleError } = useErrorHandler();
    const [isBackendReachable] = useAtom(IsBackendReachableAtom);
    const [properties, setProperties] = useAtom(PaperPropertiesSummaryAtom);

    useEffect(() => {
        const fetchProperties = async () => {
            if (!isBackendReachable) return;

            api.paper.allProperties()
                .then((response) => {
                    setProperties(response.data ?? []);
                })
                .catch(handleError);
        };

        fetchProperties();
    }, [isBackendReachable, refresh]);

    return properties;
}