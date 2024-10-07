import axios from "axios";
import { useAtom } from "jotai";
import { baseUrl } from "../http";
import { IsBackendReachableAtom } from "../atoms/IsBackendReachableAtom";

export const useBackendHealthChecker = () => {
    const [isBackendReachable, setIsBackendReachable] = useAtom(IsBackendReachableAtom);

    const checkBackendHealth = async () => {
        axios.get(`${baseUrl}/health`)
            .then(() => {
                if (!isBackendReachable) setIsBackendReachable(true);
            })
            .catch(() => {
                if (isBackendReachable) setIsBackendReachable(false);
            });
    };
    
    return { checkBackendHealth, isBackendReachable };
};