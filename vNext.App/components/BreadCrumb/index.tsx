import classNames from 'classnames';

import { Link } from '@components/Link';
import { SVGIcon } from '@components/SVGIcon';
import { capitalise } from '@helpers/formatters/capitalise';

import { Props } from './interfaces';

export const BreadCrumb: (props: Props) => JSX.Element = ({
    pathElementList,
    content,
    seperatorIconName = 'icon-chevron-right',
    truncationMinPathLength,
    truncationStartIndex,
    truncationEndIndex,
    className
}) => {

    const { ariaLabelText, truncationText } = content ?? {};

    const hasBreadCrumbElements: boolean = pathElementList.length > 0;

    const generatedClasses: any = {
        wrapper: classNames('c-breadcrumb', className),
        list: classNames('c-breadcrumb_list'),
        item: classNames('c-breadcrumb_item'),
        link: classNames('c-breadcrumb_link'),
        seperatorIcon: classNames('c-breadcrumb_icon')
    };

    const truncationTextToUse: string = truncationText ?? '...';
    const isTruncateable: boolean = 
            Number.isInteger(truncationMinPathLength) 
            && pathElementList.length > truncationMinPathLength
            && Number.isInteger(truncationStartIndex) 
            && truncationStartIndex > -1 
            && Number.isInteger(truncationEndIndex) 
            && truncationEndIndex > truncationStartIndex;

    let href: string = '';

    return (

        <nav className={generatedClasses.wrapper} aria-label={ariaLabelText}>
            {hasBreadCrumbElements &&
                <ol className={generatedClasses.list}>
                    {pathElementList.map(({ element, text }, index) => {

                        if(isTruncateable && index > truncationStartIndex && index < truncationEndIndex){

                            return null

                        }

                        const shouldRenderSeperator: boolean = index < pathElementList.length - 1;
                        const textToUse: string = (isTruncateable && index === truncationStartIndex) ? truncationTextToUse : text ? capitalise()(text) : capitalise()(element);
                        
                        href += href === '/' ? `${element}` : `/${element}`;

                        return (

                            <li key={index} className={generatedClasses.item}>
                                <Link href={href}>
                                    <a className={generatedClasses.link}>{textToUse}</a>
                                </Link>
                                {shouldRenderSeperator &&
                                    <SVGIcon name={seperatorIconName} className={generatedClasses.seperatorIcon} />
                                }
                            </li>

                        )

                    })}
                </ol>
            }
        </nav>

    )

}
