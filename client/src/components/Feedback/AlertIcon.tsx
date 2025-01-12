﻿import { Toast } from "react-hot-toast";

interface AlertIconProps {
    type: Toast['type'];
}

export default function AlertIcon({ type }: AlertIconProps) {
    let pathData;

    switch (type) {
        case 'success':
            pathData = "M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z";
            break;
        case 'error':
            pathData = "M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z";
            break;
        case 'loading':
        case 'blank':
        default:
            pathData = "M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z";
            break;
    }

    return (
        <svg
            xmlns="http://www.w3.org/2000/svg"
            className="h-6 w-6 shrink-0 stroke-current"
            fill="none"
            viewBox="0 0 24 24"
        >
            <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="2"
                d={pathData}
            />
        </svg>
    );
};