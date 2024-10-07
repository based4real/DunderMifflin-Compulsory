import { useEffect, useState } from 'react';
import { useBetween } from 'use-between';

const useShopState = () =>
{
    const [papers, setPapers] = useState("");
    const [pageInfo, setPageInfo] = useState([]);
    const [selectedProperties, setSelectProperties] = useState([]);

    const fetchPapers = () => {
        setPapers("hello")
        console.log("hello")
    }

    const enellerandenAction = (nigger: any) => {
        console.log(nigger)
    }

    useEffect(() => {
        fetchPapers();
    }, [])

    return {papers, pageInfo, selectedProperties, enellerandenAction}
}

export const useShop = () => useBetween(useShopState);