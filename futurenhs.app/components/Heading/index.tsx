import { Props } from './interfaces';

/**
 * Read only heading component for use with Redux Form
 */
export const Heading: Function = ({ 
    level = 3, 
    className, 
    children 
}: Props) => {

    let clampedLevel: number = level;

    /**
     * Clamp to valid HTML heading level
     */
    clampedLevel = level < 1 ? 1 : clampedLevel;
    clampedLevel = level > 6 ? 6 : clampedLevel;

    const HeadingTag: any = `h${clampedLevel}`;

    return (

        <HeadingTag className={className}>{children}</HeadingTag>

    );

}