import { useState } from 'react'
import classNames from 'classnames'

import { formTypes } from '@constants/forms'
import { useTheme } from '@hooks/useTheme'
import { useFormConfig } from '@hooks/useForm'
import { useCsrf } from '@hooks/useCsrf'
import { Heading } from '@components/Heading'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { Form } from '@components/Form'
import { SVGIcon } from '@components/SVGIcon'
import { Theme } from '@appTypes/theme'
import { FormConfig } from '@appTypes/form'

import { Props } from './interfaces'

export const KeyLinksBlock: (props: Props) => JSX.Element = ({
    id,
    block,
    headingLevel,
    themeId,
    isEditable,
    changeAction,
    initialErrors,
    className,
}) => {

    const [childBlocks, setChildBlocks] = useState(block.content.links);
    const { title, links } = block?.content ?? {};
    
    const hasLinks: boolean = block?.content?.links?.length > 0;

    const { background, content: contentTheme }: Theme = useTheme(themeId);
    const csrfToken: string = useCsrf();
    const formConfig: FormConfig = useFormConfig(formTypes.CONTENT_BLOCK_QUICK_LINKS_WRAPPER, {
        [`title-${id}`]: title
    }, initialErrors);

    const generatedClasses: any = {
        wrapper: classNames(className),
        heading: classNames('nhsuk-heading-m', 'u-mb-3'),
        list: classNames('u-list-none u-p-0 u-w-full'),
        item: classNames('u-mb-0'),
        link: classNames('u-block c-button c-button--themeable u-w-full u-drop-shadow u-mb-3', `u-bg-theme-${background}`, `u-text-theme-${contentTheme}`)
    };

    const handleAddChildBlock = (): void => {

        const updatedChildBlocks: Array<{
            url: string;
            linkText: string;
        }> = [...childBlocks];

        updatedChildBlocks.push({
            url: null,
            linkText: null
        });

        setChildBlocks(updatedChildBlocks)

    }

    const handleChange = (formState: Record<any, any>): void => {

        changeAction?.({ instanceId: id, formState })

    }

    if (isEditable) {

        return (

            <LayoutColumnContainer>
                <LayoutColumn desktop={9}>
                    <Form
                        key={id}
                        csrfToken={csrfToken}
                        instanceId={id}
                        formConfig={formConfig}
                        shouldRenderSubmitButton={false}
                        changeAction={handleChange} />
                </LayoutColumn>
                <LayoutColumn>
                    <ul className={generatedClasses.list}>
                        {links?.map((link, index) => {

                            const { content } = link;

                            const formConfig: FormConfig = useFormConfig(formTypes.CONTENT_BLOCK_QUICK_LINK, {
                                title: content?.linkText,
                                link: content?.url
                            });

                            const shouldRenderMovePrevious: boolean = index > 0;
                            const shouldRenderMoveNext: boolean = index < links.length - 1;

                            return (

                                <li key={index} className={generatedClasses.item}>
                                    <LayoutColumnContainer key={index} className="u-items-end">
                                        <LayoutColumn desktop={10}>
                                            <Form
                                                key={id}
                                                csrfToken={csrfToken}
                                                instanceId={id}
                                                formConfig={formConfig}
                                                shouldRenderSubmitButton={false}
                                                changeAction={handleChange}
                                                bodyClassName="u-flex u-items-end" />
                                        </LayoutColumn>
                                        <LayoutColumn desktop={2} className="u-flex u-mb-6 u-h-10 u-fill-theme-8">
                                            <div className="u-flex u-flex-col u-w-4 u-mr-5">
                                                {shouldRenderMovePrevious &&
                                                    <button type="button" className="o-link-button u-mb-2" aria-label="Move link up">
                                                        <SVGIcon name="icon-chevron-up" className="u-w-4 u-h-4" />
                                                    </button>
                                                }
                                                {shouldRenderMoveNext &&
                                                    <button type="button" className="o-link-button" aria-label="Move link down">
                                                        <SVGIcon name="icon-chevron-down" className="u-w-4 u-h-4" />
                                                    </button>
                                                }
                                            </div>
                                            <button type="button" className="o-link-button u-text-size-standard">
                                                <SVGIcon name="icon-delete" className="u-w-5 u-h-5 u-mr-2 u-align-middle" />
                                                <span className="u-align-middle">Delete link</span>
                                            </button>
                                        </LayoutColumn>
                                    </LayoutColumnContainer>
                                </li>

                            )


                        })}
                    </ul>
                </LayoutColumn>
                <LayoutColumn>
                    <button onClick={handleAddChildBlock} className="c-button c-button--secondary">Add link</button>
                </LayoutColumn>
            </LayoutColumnContainer>

        )

    }

    return (
        <div id={id} className={generatedClasses.wrapper}>
            <Heading headingLevel={headingLevel} className={generatedClasses.heading}>{title}</Heading>
            {hasLinks &&
                <LayoutColumnContainer>
                    <ul className={generatedClasses.list}>
                        {links?.map(({ content }, index) => content ? (
                            <li key={index} className={generatedClasses.item}>
                                <LayoutColumn tablet={6} desktop={4}>
                                    <a href={content.url} className={generatedClasses.link}>{content.linkText}</a>
                                </LayoutColumn>
                            </li>
                        ) : null)}
                    </ul>
                </LayoutColumnContainer>
            }
        </div>
    )
}
