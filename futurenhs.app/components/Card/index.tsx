import { useCallback } from 'react';
import { useRouter } from 'next/router';
import classNames from 'classnames';
import Image from 'next/image';

import { Props } from './interfaces';

export const Card: (props: Props) => JSX.Element = ({
    image,
    children,
    clickableHref,
    className
}) => {

    const router = useRouter();

    const { src, altText, height, width } = image ?? {};

    const generatedClasses: any = {
        wrapper: classNames('nhsuk-card', className, {
            ['nhsuk-card--clickable']: clickableHref
        }),
        hero: classNames('c-card_hero'),
        heroBody: classNames('c-card_hero-body'),
        body: classNames('nhsuk-card__content')
    };

    const handleClick = useCallback((event) => {

        if(clickableHref && !event.target.href){

            router.push(clickableHref);

        }

    }, [clickableHref]);

    return (

        <div className={generatedClasses.wrapper} onClick={handleClick}>
            {image &&
                <div className={generatedClasses.hero}>
                    <div className={generatedClasses.heroBody}>
                        <Image 
                            src={src} 
                            alt={altText} 
                            height={height} 
                            width={width} />
                    </div>
                </div>
            }
            <div className={generatedClasses.body}>
                {children}
            </div>
        </div>

    );
}