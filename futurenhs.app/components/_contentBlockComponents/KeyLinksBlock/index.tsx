import classNames from 'classnames'
import { Heading } from '@components/Heading'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { selectTheme } from '@selectors/themes'
import { themes } from '@constants/themes'
import { Theme } from '@appTypes/theme'

import { Props } from './interfaces'

export const KeyLinksBlock: (props: Props) => JSX.Element = ({
    id,
    text,
    headingLevel,
    themeId,
    links = [],
    className,
}) => {

    const { heading } = text ?? {};

    const hasLinks: boolean = links.length > 0;
    const { background, content }: Theme = selectTheme(themes, themeId);

    const generatedClasses: any = {
        wrapper: classNames(className),
        heading: classNames('nhsuk-heading-m'),
        list: classNames('u-list-none u-p-0 u-w-full'),
        item: classNames('u-m-0'),
        link: classNames('u-block c-button c-button-outline u-w-full u-drop-shadow', `u-bg-theme-${background}`, `!u-text-theme-${content}`, `u-!text-theme-${content}`)
    };

    return (
        <div id={id} className={generatedClasses.wrapper}>
            <Heading headingLevel={headingLevel} className={generatedClasses.heading}>{heading}</Heading>
            {hasLinks &&
                <LayoutColumnContainer>
                    <ul className={generatedClasses.list}>
                        {links.map(({ url, text }, index) => (
                            <li key={index} className={generatedClasses.item}>
                                <LayoutColumn tablet={6} desktop={4}>
                                    <a href={url} className={generatedClasses.link}>{text}</a>
                                </LayoutColumn>
                            </li>
                        ))}
                    </ul>
                </LayoutColumnContainer>
            }
        </div>
    )
}
