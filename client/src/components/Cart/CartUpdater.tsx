import { useAtom } from 'jotai';
import { useEffect } from 'react';
import { CartAtom } from '../../atoms/CartAtom';
import { api } from '../../http';
import { toast } from 'react-hot-toast';
import { CartOrderEntry } from '../../model/CartModel';

export default function CartUpdater() {
    const [cart, setCart] = useAtom(CartAtom);

    useEffect(() => {
        const fetchLatestPapers = async () => {
            if (cart.cartEntries.length === 0) return;

            const paperIds = cart.cartEntries.map((entry) => entry.paper.id);

            try {
                const response = await api.paper.getByIds(paperIds);
                const latestPapers = response.data;

                const updatedCartEntries = cart.cartEntries
                    .map((entry) => {
                        const latestPaper = latestPapers.find((p) => p.id === entry.paper.id);
                        if (latestPaper && !latestPaper.discontinued) {
                            return { ...entry, paper: latestPaper };
                        } else {
                            return null;
                        }
                    })
                    .filter((entry) => entry !== null) as CartOrderEntry[];

                if (updatedCartEntries.length !== cart.cartEntries.length) {
                    toast.error('Some items in your cart have been discontinued and were removed.');
                }

                const updatedCart = { ...cart, cartEntries: updatedCartEntries };
                setCart(updatedCart);
                localStorage.setItem('cartItems', JSON.stringify(updatedCart));
            } catch (error) {
                console.error('Error fetching latest papers:', error);
            }
        };

        fetchLatestPapers();
    }, []);

    return null;
}