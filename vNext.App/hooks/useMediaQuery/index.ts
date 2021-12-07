import { useEffect, useState } from 'react';

export const useMediaQuery = (query: string): boolean => {

    const [matches, setMatches] = useState(false);

    useEffect(() => {

        const media: any = window.matchMedia(query);

        if (media.matches !== matches) {

            setMatches(media.matches);

        }

        const listener = () => {

            setMatches(media.matches);

        };

        media.addListener(listener);

        return () => media.removeListener(listener);

    }, [matches, query]);

    return matches;

}